
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    [Header("Configurações")]
    [SerializeField] private double startLeadTime = 1.0;

    private double scheduledStartTime;
    private bool musicScheduled;

    // Retorna a posição atual da música em segundos.
    public double SongTime
    {
        get
        {
            if (!musicScheduled)
                return -1.0;

            return AudioSettings.dspTime - scheduledStartTime;
        }
    }

    // Informa se o instante inicial da música já chegou.
    public bool HasStarted
    {
        get
        {
            return musicScheduled &&
                   AudioSettings.dspTime >= scheduledStartTime;
        }
    }

    public void StartMusic()
    {
        if (musicSource == null)
        {
            Debug.LogError("MusicManager: AudioSource não atribuído!");
            return;
        }

        if (musicSource.clip == null)
        {
            Debug.LogError("MusicManager: Nenhuma música atribuída!");
            return;
        }

        musicSource.Stop();
        musicSource.time = 0f;

        // Agenda o início da música.
        scheduledStartTime =
            AudioSettings.dspTime + startLeadTime;

        musicScheduled = true;

        musicSource.PlayScheduled(scheduledStartTime);

        Debug.Log("Música agendada!");
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicScheduled = false;
    }
}
