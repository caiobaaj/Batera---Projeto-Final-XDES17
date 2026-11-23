using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PecaBateria : MonoBehaviour
{
    [Header("Precisão em segundos")]
    [SerializeField] private float folgaTempoPerfeito = 0.05f;
    [SerializeField] private float folgaTempoBom = 0.10f;
    [SerializeField] private float folgaTempoRuim = 0.15f;

    [Header("Input Principal")]
    [SerializeField] private InputActionReference acaoToque;

    [Header("Visual (Objeto PLAY)")]
    [SerializeField] private SpriteRenderer spritePlay;
    [SerializeField] private float tempoPiscada = 0.08f;
    private Color _corOriginal;

    [Header("Cores de Feedback por Precisão")]
    [SerializeField] private Color corToqueLivre = new Color(0.86f, 0.08f, 0.24f, 1f);
    [SerializeField] private Color corPerfeito = Color.cyan;
    [SerializeField] private Color corBom = Color.green;
    [SerializeField] private Color corRuim = Color.yellow;
    [SerializeField] private Color corMiss = Color.red;

    [Header("Sistema de Auréola")]
    [SerializeField] private AureolaController prefabAureola;
    [SerializeField] private Sprite spriteAureolaEspecifico;
    [SerializeField] private int tamanhoPoolAureolas = 8;
    [SerializeField] private bool mostrarLogsNoConsole = true;

    [Header("Áudio Principal")]
    [SerializeField] private AudioClip somPadrao;
    [SerializeField] private float pitchMin = 0.89f;
    [SerializeField] private float pitchMax = 0.91f;

    [Header("Modificador de Pedal (Apenas para o Hi-Hat)")]
    [SerializeField] private InputActionReference acaoModificadorPedal;
    [SerializeField] private AudioClip somComPedalPressionado;
    [SerializeField] private bool cortarSomAbertoAoPisarNoPedal = true;
    [Tooltip("Objeto sólido no formato do pedal do Hi-Hat que acende quando a próxima nota exige pedal segurado (HiHatClosed)")]
    [SerializeField] private GameObject objetoAvisoSegurarPedal;

    private AudioSource _audioSource;
    private Coroutine _rotinaPiscar;
    private WaitForSeconds _esperaPiscada;
    private bool _ultimoSomFoiAberto;

    private readonly Queue<AureolaController> _filaAureolasAtivas = new Queue<AureolaController>();
    private readonly Queue<AureolaController> _poolAureolasInativas = new Queue<AureolaController>();

    private bool UsaModificadorDePedal => acaoModificadorPedal != null && acaoModificadorPedal.action != null;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _esperaPiscada = new WaitForSeconds(tempoPiscada);

        corToqueLivre.a = 1f;
        corPerfeito.a = 1f;
        corBom.a = 1f;
        corRuim.a = 1f;
        corMiss.a = 1f;

        if (spritePlay == null)
        {
            Transform filhoPlay = transform.Find("PLAY");
            if (filhoPlay != null)
                spritePlay = filhoPlay.GetComponent<SpriteRenderer>();
        }

        if (spritePlay != null)
        {
            _corOriginal = spritePlay.color;
        }

        if (objetoAvisoSegurarPedal != null)
        {
            objetoAvisoSegurarPedal.SetActive(false);
        }

        InicializarPool();
    }

    private void InicializarPool()
    {
        if (prefabAureola == null || spritePlay == null) return;

        for (int i = 0; i < tamanhoPoolAureolas; i++)
        {
            AureolaController aureola = Instantiate(prefabAureola, spritePlay.transform);
            aureola.ConfigurarPool(this, spritePlay, spriteAureolaEspecifico);
            _poolAureolasInativas.Enqueue(aureola);
        }
    }

    private void OnEnable()
    {
        if (acaoToque != null && acaoToque.action != null)
        {
            acaoToque.action.Enable();
            acaoToque.action.performed += OnToquePerformed;
        }

        if (acaoModificadorPedal != null && acaoModificadorPedal.action != null)
        {
            acaoModificadorPedal.action.Enable();
            acaoModificadorPedal.action.performed += OnPedalModificadorPerformed;
        }
    }

    private void OnDisable()
    {
        if (acaoToque != null && acaoToque.action != null)
            acaoToque.action.performed -= OnToquePerformed;

        if (acaoModificadorPedal != null && acaoModificadorPedal.action != null)
            acaoModificadorPedal.action.performed -= OnPedalModificadorPerformed;
    }

    public void CriarAureola(double tempoAlvoDSP, float tempoAntecedencia, bool exigePedalSegurado = false)
    {
        if (prefabAureola == null || spritePlay == null) return;

        AureolaController novaAureola;
        if (_poolAureolasInativas.Count > 0)
        {
            novaAureola = _poolAureolasInativas.Dequeue();
        }
        else
        {
            novaAureola = Instantiate(prefabAureola, spritePlay.transform);
            novaAureola.ConfigurarPool(this, spritePlay, spriteAureolaEspecifico);
        }

        bool destacarHiHatAberto = UsaModificadorDePedal && !exigePedalSegurado;

        novaAureola.AtivarNota(tempoAlvoDSP, tempoAntecedencia, exigePedalSegurado, destacarHiHatAberto);
        _filaAureolasAtivas.Enqueue(novaAureola);

        AtualizarAvisoVisualDoPedal();
    }

    public void NotificarMissPorTempo(AureolaController aureolaExpirada)
    {
        if (_filaAureolasAtivas.Count > 0 && _filaAureolasAtivas.Peek() == aureolaExpirada)
        {
            _filaAureolasAtivas.Dequeue();
        }

        aureolaExpirada.DesativarNota();
        _poolAureolasInativas.Enqueue(aureolaExpirada);

        AtualizarAvisoVisualDoPedal();

        if (mostrarLogsNoConsole)
            Debug.Log($"[{gameObject.name}] MISS!");

        PiscarCor(corMiss);
    }

    public void LimparAureolasAtivas()
    {
        while (_filaAureolasAtivas.Count > 0)
        {
            AureolaController aureola = _filaAureolasAtivas.Dequeue();
            aureola.DesativarNota();
            _poolAureolasInativas.Enqueue(aureola);
        }

        if (objetoAvisoSegurarPedal != null)
            objetoAvisoSegurarPedal.SetActive(false);

        if (_rotinaPiscar != null)
        {
            StopCoroutine(_rotinaPiscar);
            _rotinaPiscar = null;
        }

        if (spritePlay != null)
            spritePlay.color = _corOriginal;
    }

    private void AtualizarAvisoVisualDoPedal()
    {
        if (objetoAvisoSegurarPedal == null) return;

        bool deveSegurarPedalAgora = _filaAureolasAtivas.Count > 0 && _filaAureolasAtivas.Peek().ExigePedalSegurado;
        objetoAvisoSegurarPedal.SetActive(deveSegurarPedalAgora);
    }

    private void OnToquePerformed(InputAction.CallbackContext context)
    {
        if (!context.control.IsPressed()) return;

        bool pedalSegurado = UsaModificadorDePedal && acaoModificadorPedal.action.IsPressed();

        AudioClip clipParaTocar = (pedalSegurado && somComPedalPressionado != null) 
            ? somComPedalPressionado 
            : somPadrao;

        _ultimoSomFoiAberto = !pedalSegurado;

        TocarSom(clipParaTocar);
        Color corFeedback = AvaliarPrecisaoToque(pedalSegurado);
        PiscarCor(corFeedback);
    }

    private Color AvaliarPrecisaoToque(bool pedalEstavaSegurado)
    {
        if (_filaAureolasAtivas.Count == 0)
        {
            return corToqueLivre;
        }

        AureolaController notaMaisAntiga = _filaAureolasAtivas.Peek();

        double tempoAtual = MusicaController.Instancia != null
            ? MusicaController.Instancia.TempoReferenciaAtual
            : AudioSettings.dspTime;

        double diferenca = Math.Abs(tempoAtual - notaMaisAntiga.TempoAlvo);

        if (diferenca > folgaTempoRuim)
        {
            return corToqueLivre;
        }

        _filaAureolasAtivas.Dequeue();
        notaMaisAntiga.DesativarNota();
        _poolAureolasInativas.Enqueue(notaMaisAntiga);

        AtualizarAvisoVisualDoPedal();

        if (UsaModificadorDePedal && pedalEstavaSegurado != notaMaisAntiga.ExigePedalSegurado)
        {
            if (mostrarLogsNoConsole) Debug.Log($"[{gameObject.name}] PEDAL INCORRETO -> RUIM");
            return corRuim;
        }

        if (diferenca <= folgaTempoPerfeito)
        {
            if (mostrarLogsNoConsole) Debug.Log($"[{gameObject.name}] PERFEITO! ({diferenca * 1000.0:F0}ms)");
            return corPerfeito;
        }

        if (diferenca <= folgaTempoBom)
        {
            if (mostrarLogsNoConsole) Debug.Log($"[{gameObject.name}] BOM! ({diferenca * 1000.0:F0}ms)");
            return corBom;
        }

        if (mostrarLogsNoConsole) Debug.Log($"[{gameObject.name}] RUIM! ({diferenca * 1000.0:F0}ms)");
        return corRuim;
    }

    private void OnPedalModificadorPerformed(InputAction.CallbackContext context)
    {
        if (!context.control.IsPressed()) return;

        if (cortarSomAbertoAoPisarNoPedal && _ultimoSomFoiAberto && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    private void TocarSom(AudioClip som)
    {
        if (som != null)
        {
            _audioSource.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
            _audioSource.PlayOneShot(som);
        }
    }

    private void PiscarCor(Color corParaPiscar)
    {
        if (spritePlay == null) return;

        if (_rotinaPiscar != null)
            StopCoroutine(_rotinaPiscar);

        _rotinaPiscar = StartCoroutine(CorPiscar(corParaPiscar));
    }

    private IEnumerator CorPiscar(Color corAlvo)
    {
        spritePlay.color = corAlvo;
        yield return _esperaPiscada;
        spritePlay.color = _corOriginal;
        _rotinaPiscar = null;
    }
}