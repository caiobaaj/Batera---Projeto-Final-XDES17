using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MusicManager musicManager;

    private void Start()
    {
        musicManager.StartMusic();
    }
}