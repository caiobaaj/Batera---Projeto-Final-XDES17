using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class MusicaController : MonoBehaviour
{
    public static MusicaController Instancia { get; private set; }

    [Header("UI do Menu Rápido")]
    [Tooltip("Arraste o Panel do Menu Inicial para cá")]
    [SerializeField] private GameObject painelMenuInicial;

    [Header("Beatmap da Música")]
    [SerializeField] private MapaMusica mapaAtual;
    [SerializeField] private double atrasoInicialSegundos = 1.5;

    [Header("Referências das 9 Peças na Cena (Hierarchy)")]
    [SerializeField] private PecaBateria pecaKick;
    [SerializeField] private PecaBateria pecaSnare;
    [SerializeField] private PecaBateria pecaHighTom;
    [SerializeField] private PecaBateria pecaLowTom;
    [SerializeField] private PecaBateria pecaFloorTom;
    [SerializeField] private PecaBateria pecaHiHat;
    [SerializeField] private PecaBateria pecaHiHatPedal;
    [SerializeField] private PecaBateria pecaCrash;
    [SerializeField] private PecaBateria pecaRide;

    private AudioSource _audioSource;
    private double _dspTimeInicioMusica;
    private double _segundosPorBatida;
    private int _indiceProximaNota;
    private bool _jogoIniciado = false;

    public double TempoMusicaAtual => _jogoIniciado ? (AudioSettings.dspTime - _dspTimeInicioMusica) : -999.0;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        _audioSource = GetComponent<AudioSource>();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
    }

    private void Start()
    {
        // Se você não arrastar nenhum painel de menu, ele começa direto para não quebrar seus testes
        if (painelMenuInicial == null)
        {
            IniciarJogoPeloMenu();
        }
        else
        {
            painelMenuInicial.SetActive(true);
        }
    }

    // VINCULE ESTE MÉTODO NO "ON CLICK()" DO SEU BOTÃO JOGAR (COM MÚSICA)!
     public void IniciarJogoPeloMenu()
    {
        if (_jogoIniciado) return;

        if (painelMenuInicial != null)
            painelMenuInicial.SetActive(false);

        if (mapaAtual == null)
        {
            Debug.LogError("Nenhum mapa de música foi configurado!");
            return;
        }

        if (mapaAtual.bpm <= 0)
        {
            Debug.LogError("O BPM precisa ser maior que zero!");
            return;
        }

        if (mapaAtual.arquivoAudio == null)
        {
            Debug.LogError("O mapa não possui uma música configurada!");
            return;
        }

        if (mapaAtual.tempoAntecedenciaAureola <= 0)
        {
            Debug.LogError("O tempo de antecedência precisa ser maior que zero!");
            return;
        }

        mapaAtual.OrdenarNotas();

        _segundosPorBatida = 60.0 / mapaAtual.bpm;
        _indiceProximaNota = 0;
        _dspTimeInicioMusica = AudioSettings.dspTime + atrasoInicialSegundos;

        _audioSource.clip = mapaAtual.arquivoAudio;
        _audioSource.PlayScheduled(_dspTimeInicioMusica);
        

        _jogoIniciado = true;
    }

    // VINCULE ESTE MÉTODO NO "ON CLICK()" DO SEU BOTÃO MODO LIVRE (SEM MÚSICA)!
    public void IniciarModoLivrePeloMenu()
    {
        if (painelMenuInicial != null)
            painelMenuInicial.SetActive(false);

        _jogoIniciado = false;
    }

    public void ReiniciarJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        // Atalho de emergência na aula: aperte BACKSPACE para voltar ao menu/reiniciar!
        if (Keyboard.current != null && Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            ReiniciarJogo();
            return;
        }

        if (!_jogoIniciado || mapaAtual == null || mapaAtual.notas == null) return;

        while (_indiceProximaNota < mapaAtual.notas.Count)
        {
            NotaMapa nota = mapaAtual.notas[_indiceProximaNota];
            double tempoAlvoDSP = mapaAtual.offsetPrimeiroBeatSegundos + (nota.batida * _segundosPorBatida);

            if (TempoMusicaAtual >= tempoAlvoDSP - mapaAtual.tempoAntecedenciaAureola)
            {
                DispararNotaNaPeca(nota.tipoNota, tempoAlvoDSP, mapaAtual.tempoAntecedenciaAureola);
                _indiceProximaNota++;
            }
            else
            {
                break;
            }
        }
    }

    private void DispararNotaNaPeca(TipoNotaBateria tipoNota, double tempoAlvo, float antecedencia)
    {
        switch (tipoNota)
        {
            case TipoNotaBateria.Kick:
                if (pecaKick != null) pecaKick.CriarAureola(tempoAlvo, antecedencia);
                break;
            case TipoNotaBateria.Snare:
                if (pecaSnare != null) pecaSnare.CriarAureola(tempoAlvo, antecedencia);
                break;
            case TipoNotaBateria.HighTom:
                if (pecaHighTom != null) pecaHighTom.CriarAureola(tempoAlvo, antecedencia);
                break;
            case TipoNotaBateria.LowTom:
                if (pecaLowTom != null) pecaLowTom.CriarAureola(tempoAlvo, antecedencia);
                break;
            case TipoNotaBateria.FloorTom:
                if (pecaFloorTom != null) pecaFloorTom.CriarAureola(tempoAlvo, antecedencia);
                break;
            case TipoNotaBateria.HiHatClosed:
                if (pecaHiHat != null) pecaHiHat.CriarAureola(tempoAlvo, antecedencia, exigePedalSegurado: true);
                break;
            case TipoNotaBateria.HiHatOpen:
                if (pecaHiHat != null) pecaHiHat.CriarAureola(tempoAlvo, antecedencia, exigePedalSegurado: false);
                break;
            case TipoNotaBateria.HiHatPedal:
                if (pecaHiHatPedal != null) pecaHiHatPedal.CriarAureola(tempoAlvo, antecedencia);
                break;
            case TipoNotaBateria.Crash:
                if (pecaCrash != null) pecaCrash.CriarAureola(tempoAlvo, antecedencia);
                break;
            case TipoNotaBateria.Ride:
                if (pecaRide != null) pecaRide.CriarAureola(tempoAlvo, antecedencia);
                break;
        }
    }
}