using UnityEngine;

public class AureolaController : MonoBehaviour
{
    [Header("Escala Visual (Baseada no tamanho real do PLAY)")]
    [SerializeField] private float multiplicadorEscalaInicial = 3.0f;
    [SerializeField] private float multiplicadorEscalaAlvo = 1.0f;
    [SerializeField] private float multiplicadorEscalaMinimaMiss = 0.55f;
    [SerializeField] private float ajusteFinoBorda = 1.0f;

    [Header("Cores da Auréola (Normal vs Hi-Hat Aberto)")]
    [SerializeField] private Color corAureolaPadrao = Color.white;
    [Tooltip("Cor da auréola quando for Hi-Hat Open (soltar o pedal), para diferenciar facilmente!")]
    [SerializeField] private Color corAureolaHiHatAberto = new Color(0.3f, 0.85f, 1f, 1f);

    public double TempoAlvo { get; private set; }
    public float EscalaRelativaAtual { get; private set; }
    public bool ExigePedalSegurado { get; private set; }

    private double _tempoSpawn;
    private float _duracaoAntecedencia;
    private PecaBateria _pecaDona;
    private Vector3 _escalaEncaixePerfeito;
    private SpriteRenderer _spriteRenderer;
    private Color _corBaseAtual;
    private bool _ativa;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ConfigurarPool(PecaBateria peca, SpriteRenderer spritePlay, Sprite spriteAureolaCustom = null)
    {
        _pecaDona = peca;

        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteAureolaCustom != null && _spriteRenderer != null)
            _spriteRenderer.sprite = spriteAureolaCustom;

        if (_spriteRenderer != null && _spriteRenderer.sprite != null && spritePlay.sprite != null)
        {
            Vector2 tamanhoSpritePlay = spritePlay.sprite.bounds.size;
            Vector2 tamanhoSpriteAureola = _spriteRenderer.sprite.bounds.size;

            float escalaX = (tamanhoSpritePlay.x / tamanhoSpriteAureola.x) * ajusteFinoBorda;
            float escalaY = (tamanhoSpritePlay.y / tamanhoSpriteAureola.y) * ajusteFinoBorda;

            _escalaEncaixePerfeito = new Vector3(escalaX, escalaY, 1f);
        }
        else
        {
            _escalaEncaixePerfeito = Vector3.one;
        }

        gameObject.SetActive(false);
    }

    public void AtivarNota(double tempoAlvo, float duracaoAntecedencia, bool exigePedalSegurado = false, bool destacarComoHiHatAberto = false)
    {
        TempoAlvo = tempoAlvo;
        _duracaoAntecedencia = duracaoAntecedencia;
        _tempoSpawn = tempoAlvo - duracaoAntecedencia;
        ExigePedalSegurado = exigePedalSegurado;
        EscalaRelativaAtual = multiplicadorEscalaInicial;

        _corBaseAtual = destacarComoHiHatAberto ? corAureolaHiHatAberto : corAureolaPadrao;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = _escalaEncaixePerfeito * multiplicadorEscalaInicial;

        _ativa = true;
        gameObject.SetActive(true);
    }

    public void DesativarNota()
    {
        _ativa = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_ativa || MusicaController.Instancia == null) return;

        double tempoAtual = MusicaController.Instancia.TempoMusicaAtual;
        float progresso = (float)((tempoAtual - _tempoSpawn) / _duracaoAntecedencia);

        EscalaRelativaAtual = Mathf.LerpUnclamped(multiplicadorEscalaInicial, multiplicadorEscalaAlvo, progresso);
        transform.localScale = _escalaEncaixePerfeito * Mathf.Max(0f, EscalaRelativaAtual);

        if (_spriteRenderer != null)
        {
            Color c = _corBaseAtual;
            c.a = Mathf.Clamp01(progresso * 4f);
            _spriteRenderer.color = c;
        }

        if (EscalaRelativaAtual <= multiplicadorEscalaMinimaMiss)
        {
            _pecaDona.NotificarMissPorTempo(this);
        }
    }
}