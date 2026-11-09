using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
//using Minis; 

public class ControlesBateria : MonoBehaviour
{
    private BateriaActionsScript _controles;
    
    [Header("Audio")]
    [SerializeField] private AudioClip somSnare;
    [SerializeField] private float pitchMax;
    [SerializeField] private float pitchMin;
    private AudioSource _audioSource;
    
    [Header("Piscar Cor")]
    [SerializeField] private Color corPiscada = Color.crimson;
    [SerializeField] private float tempoPiscada;
    private Color _corOriginal;
    private Transform _objetoSnare;
    private SpriteRenderer _spriteSnare;
    private Coroutine _routineSnare;
    
    private void Awake()
    {
        _controles = new BateriaActionsScript();
        _controles.Bateria.Snare.performed += OnSnarePerformed;
        
        _audioSource = GetComponent<AudioSource>();
        
        _objetoSnare = transform.Find("Snare/PLAY");
        _spriteSnare = _objetoSnare.GetComponent<SpriteRenderer>();
        _corOriginal = _spriteSnare.color;
    }
    
    private void OnEnable()
    {
        _controles.Enable();
    }

    private void OnDisable()
    {
        _controles.Disable();
    }

    private void OnSnarePerformed(InputAction.CallbackContext context)
    {
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Snare();
        }
    }

    private void Snare()
    {
        if (somSnare != null && _spriteSnare != null)
        {
            _audioSource.pitch = Random.Range(pitchMin, pitchMax);
            //_audioSource.PlayOneShot(somSnare);
            _audioSource.clip = somSnare;
            _audioSource.PlayScheduled(AudioSettings.dspTime);
            if (_routineSnare != null)
                StopCoroutine(_routineSnare);
            _routineSnare = StartCoroutine(CorPiscar());
        }
    }

    private IEnumerator CorPiscar()
    {
        _spriteSnare.color = corPiscada;
        yield return new WaitForSeconds(tempoPiscada);
        _spriteSnare.color = _corOriginal;
    }
}