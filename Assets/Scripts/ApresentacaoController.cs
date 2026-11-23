using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using TMPro;

public class ApresentacaoController : MonoBehaviour
{
    [Serializable]
    public class EtapaApresentacao
    {
        public string nomeEtapa;
        [Tooltip("Painel de UI desta etapa. Deixe vazio se for apenas a bateria.")]
        public GameObject painelUI;
        [Tooltip("Quais peças da bateria devem aparecer nesta etapa")]
        public GameObject[] pecasVisiveis;
        [Tooltip("Se marcado, gera auréolas lentas em loop nas peças visíveis para demonstrar acerto/erro")]
        public bool demonstrarAureolaEmLoop;
        [Tooltip("Se marcado, libera o Menu Principal do jogo (The Chain / Modo Livre)")]
        public bool liberarMenuJogoFinal;
    }

    [Serializable]
    public class ItemReferencia
    {
        public string tituloBotao;
        public VideoClip videoGameplay;
        public Sprite logoDoJogo;
        [TextArea(4, 8)]
        public string textoFichaTecnica;
    }

    [Header("Navegação (Use Setas Esquerda / Direita)")]
    [SerializeField] private EtapaApresentacao[] etapas;
    [SerializeField] private GameObject painelMenuJogoFinal;

    [Header("Demonstração de Auréola (Etapa 3)")]
    [SerializeField] private PecaBateria[] pecasDemoAureola;
    [SerializeField] private float intervaloEntreAureolasDemo = 2.5f;
    [SerializeField] private float tempoAntecedenciaDemo = 1.8f;

    [Header("Etapa 4: Jogos de Referência (4 Vídeos + Logo + Texto)")]
    [SerializeField] private VideoPlayer videoPlayerReferencia;
    [SerializeField] private Image imagemLogoReferencia;
    [SerializeField] private TMP_Text textoApoioReferencia;
    [SerializeField] private ItemReferencia[] itensReferencia; // 0 = GuitarHero, 1 = osu! (v1), 2 = osu! (v2), 3 = DrumMania

    private int _etapaAtual = 0;
    private double _proximoSpawnDemoDSP;
    private int _indicePecaDemo = 0;

    private void Start()
    {
        IrParaEtapa(0);
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.pageDownKey.wasPressedThisFrame)
            {
                AvancarEtapa();
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.pageUpKey.wasPressedThisFrame)
            {
                VoltarEtapa();
            }
        }

        if (etapas != null && _etapaAtual < etapas.Length && etapas[_etapaAtual].demonstrarAureolaEmLoop)
        {
            if (AudioSettings.dspTime >= _proximoSpawnDemoDSP && pecasDemoAureola != null && pecasDemoAureola.Length > 0)
            {
                PecaBateria peca = pecasDemoAureola[_indicePecaDemo % pecasDemoAureola.Length];
                if (peca != null)
                {
                    peca.CriarAureola(AudioSettings.dspTime + tempoAntecedenciaDemo, tempoAntecedenciaDemo);
                }

                _indicePecaDemo++;
                _proximoSpawnDemoDSP = AudioSettings.dspTime + intervaloEntreAureolasDemo;
            }
        }
    }

    public void AvancarEtapa()
    {
        if (_etapaAtual < etapas.Length - 1)
            IrParaEtapa(_etapaAtual + 1);
    }

    public void VoltarEtapa()
    {
        if (_etapaAtual > 0)
            IrParaEtapa(_etapaAtual - 1);
    }

    public void IrParaEtapa(int indice)
    {
        _etapaAtual = Mathf.Clamp(indice, 0, etapas.Length - 1);

        // Para qualquer vídeo que estivesse rodando ao mudar de slide
        if (videoPlayerReferencia != null && videoPlayerReferencia.isPlaying)
        {
            videoPlayerReferencia.Stop();
        }

        for (int i = 0; i < etapas.Length; i++)
        {
            if (etapas[i].painelUI != null)
                etapas[i].painelUI.SetActive(false);

            if (etapas[i].pecasVisiveis != null)
            {
                foreach (var peca in etapas[i].pecasVisiveis)
                {
                    if (peca != null) peca.SetActive(false);
                }
            }
        }

        EtapaApresentacao atual = etapas[_etapaAtual];

        if (atual.painelUI != null)
            atual.painelUI.SetActive(true);

        if (atual.pecasVisiveis != null)
        {
            foreach (var peca in atual.pecasVisiveis)
            {
                if (peca != null) peca.SetActive(true);
            }
        }

        if (painelMenuJogoFinal != null)
            painelMenuJogoFinal.SetActive(atual.liberarMenuJogoFinal);

        if (atual.demonstrarAureolaEmLoop)
            _proximoSpawnDemoDSP = AudioSettings.dspTime + 0.5;
    }

    // VINCULE ESTE MÉTODO NOS 4 BOTÕES DA ETAPA 4 (0, 1, 2 e 3)
    public void SelecionarReferencia(int indice)
    {
        if (itensReferencia == null || indice < 0 || indice >= itensReferencia.Length)
            return;

        ItemReferencia item = itensReferencia[indice];

        if (videoPlayerReferencia != null && item.videoGameplay != null)
        {
            videoPlayerReferencia.Stop();
            videoPlayerReferencia.clip = item.videoGameplay;
            videoPlayerReferencia.Play();
        }

        if (imagemLogoReferencia != null)
        {
            imagemLogoReferencia.sprite = item.logoDoJogo;
            imagemLogoReferencia.preserveAspect = true;
            imagemLogoReferencia.enabled = (item.logoDoJogo != null);
        }

        if (textoApoioReferencia != null)
        {
            textoApoioReferencia.text = item.textoFichaTecnica;
        }
    }
}