using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum TipoNotaBateria
{
    Kick = 0,
    Snare = 1,
    HighTom = 2,
    LowTom = 3,
    FloorTom = 4,
    HiHatClosed = 5, // Bater no Hi-Hat SEGURANDO o pedal
    HiHatOpen = 6,   // Bater no Hi-Hat com o pedal SOLTO
    HiHatPedal = 7,  // Pisar no pedal no ritmo (som de "chick")
    Crash = 8,
    Ride = 9
}

[Serializable]
public struct NotaMapa
{
    [Tooltip("Momento da música em BATIDAS (ex: 1.0, 1.5, 2.0, 2.25)")]
    public float batida;
    public TipoNotaBateria tipoNota;
}

[CreateAssetMenu(fileName = "NovoMapaMusica", menuName = "Bateria/Mapa de Musica")]
public class MapaMusica : ScriptableObject
{
    [Header("Informações da Faixa")]
    public string nomeMusica = "Nova Música";
    public AudioClip arquivoAudio;
    public float bpm = 120f;

    [Tooltip("Atraso da primeira batida no arquivo de áudio (em segundos)")]
    public float offsetPrimeiroBeatSegundos = 0f;

    [Tooltip("Segundos de antecedência em que a auréola aparece (Approach Rate)")]
    public float tempoAntecedenciaAureola = 1.8f;

    [Header("Importador Rápido de Partitura (Cole do Chat aqui!)")]
    [Tooltip("Formato: Batida|Peca separados por vírgula ou linha. Ex: 1.0|Kick, 1.0|HHC, 1.5|HHO, 2.0|Snare")]
    [TextArea(5, 12)]
    public string partituraEmTexto;

    [Header("Lista de Notas Geradas")]
    public List<NotaMapa> notas = new List<NotaMapa>();

    public void OrdenarNotas()
    {
        notas.Sort((a, b) => a.batida.CompareTo(b.batida));
    }

    // Clique nos 3 pontinhos do componente no Inspector -> "Importar Partitura do Texto"
    [ContextMenu("Importar Partitura do Texto")]
    public void ImportarPartituraDoTexto()
    {
        if (string.IsNullOrWhiteSpace(partituraEmTexto)) return;

        notas.Clear();
        string[] entradas = partituraEmTexto.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string entrada in entradas)
        {
            string limpa = entrada.Trim();
            if (string.IsNullOrEmpty(limpa) || limpa.StartsWith("#") || limpa.StartsWith("//"))
                continue;

            string[] partes = limpa.Split('|');
            if (partes.Length != 2) continue;

            if (float.TryParse(partes[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float batida))
            {
                if (TentarConverterPeca(partes[1].Trim(), out TipoNotaBateria tipo))
                {
                    notas.Add(new NotaMapa { batida = batida, tipoNota = tipo });
                }
            }
        }

        OrdenarNotas();
        Debug.Log($"[MapaMusica] '{nomeMusica}' importado com sucesso! Total de notas: {notas.Count}");
    }

    private bool TentarConverterPeca(string texto, out TipoNotaBateria tipo)
    {
        switch (texto.ToUpperInvariant())
        {
            case "KICK": case "BD": case "BUMBO":
                tipo = TipoNotaBateria.Kick; return true;
            case "SNARE": case "SD": case "CAIXA":
                tipo = TipoNotaBateria.Snare; return true;
            case "HIGHTOM": case "HT": case "TOM1":
                tipo = TipoNotaBateria.HighTom; return true;
            case "LOWTOM": case "LT": case "TOM2":
                tipo = TipoNotaBateria.LowTom; return true;
            case "FLOORTOM": case "FT": case "SURDO":
                tipo = TipoNotaBateria.FloorTom; return true;
            case "HIHATCLOSED": case "HHC": case "CHH":
                tipo = TipoNotaBateria.HiHatClosed; return true;
            case "HIHATOPEN": case "HHO": case "OHH":
                tipo = TipoNotaBateria.HiHatOpen; return true;
            case "HIHATPEDAL": case "HHP": case "PHH":
                tipo = TipoNotaBateria.HiHatPedal; return true;
            case "CRASH": case "CR":
                tipo = TipoNotaBateria.Crash; return true;
            case "RIDE": case "RD":
                tipo = TipoNotaBateria.Ride; return true;
            default:
                return Enum.TryParse(texto, true, out tipo);
        }
    }
}