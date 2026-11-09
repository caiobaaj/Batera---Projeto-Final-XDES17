using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
//using Minis; 

public class ControlesBateria : MonoBehaviour
{
    private BateriaActionsScript _controles;
    
    [Header("Audio")]
    [SerializeField] private AudioClip somSnare;
    [SerializeField] private AudioClip somKick;
    [SerializeField] private AudioClip somHighTom;
    [SerializeField] private AudioClip somLowTom;
    [SerializeField] private AudioClip somFloorTom;
    [SerializeField] private AudioClip somHiHat;
    [SerializeField] private AudioClip somHiHatComPedal;
    [SerializeField] private AudioClip somPedalHiHat;
    [SerializeField] private AudioClip somCrash;
    [SerializeField] private AudioClip somRide;
    [SerializeField] private float pitchMax;
    [SerializeField] private float pitchMin;
    private AudioSource _audioSource;
    
    [Header("Piscar Cor")]
    [SerializeField] private Color corPiscada;
    [SerializeField] private float tempoPiscada;
    [SerializeField] private Color corOriginal;
    
    private Transform _objetoSnare;
    private Transform _objetoKick;
    private Transform _objetoHighTom;
    private Transform _objetoLowTom;
    private Transform _objetoFloorTom;
    private Transform _objetoHiHat;
    private Transform _objetoHiHatPedal;
    private Transform _objetoCrash;
    private Transform _objetoRide;
    
    private SpriteRenderer _sprite;
    private Coroutine _routine;
    
    private void Awake()
    {
        _controles = new BateriaActionsScript();
        
        _controles.Bateria.Snare.performed += OnSnarePerformed;
        _controles.Bateria.Kick.performed += OnKickPerformed;
        _controles.Bateria.HighTom.performed += OnHighTomPerformed;
        _controles.Bateria.LowTom.performed += OnLowTomPerformed;
        _controles.Bateria.FloorTom.performed += OnFloorTomPerformed;
        _controles.Bateria.HiHat.performed += OnHiHatPerformed;
        _controles.Bateria.HiHatPedal.performed += OnHiHatPedalPerformed;
        _controles.Bateria.Crash.performed += OnCrashPerformed;
        _controles.Bateria.Ride.performed += OnRidePerformed;
        
        _audioSource = GetComponent<AudioSource>();
        
        _objetoSnare = transform.Find("Snare/PLAY");
        _objetoKick = transform.Find("Kick/PLAY");
        _objetoHighTom = transform.Find("HighTom/PLAY");
        _objetoLowTom = transform.Find("LowTom/PLAY");
        _objetoFloorTom = transform.Find("FloorTom/PLAY");
        _objetoHiHat = transform.Find("HiHat/PLAY");
        _objetoHiHatPedal = transform.Find("HiHatPedal/PLAY");
        _objetoCrash = transform.Find("Crash/PLAY");
        _objetoRide = transform.Find("Ride/PLAY");
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
        _sprite = _objetoSnare.GetComponent<SpriteRenderer>();
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Juicyness(somSnare); 
        }
    }
    
    private void OnKickPerformed(InputAction.CallbackContext context)
    {
        _sprite = _objetoKick.GetComponent<SpriteRenderer>();
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Juicyness(somKick);
        }
    }
    
    private void OnHighTomPerformed(InputAction.CallbackContext context)
    {
        _sprite = _objetoHighTom.GetComponent<SpriteRenderer>();
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Juicyness(somHighTom);
        }
    }
    
    private void OnLowTomPerformed(InputAction.CallbackContext context)
    {
        _sprite = _objetoLowTom.GetComponent<SpriteRenderer>();
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Juicyness(somLowTom);
        }
    }
    
    private void OnFloorTomPerformed(InputAction.CallbackContext context)
    {
        _sprite = _objetoFloorTom.GetComponent<SpriteRenderer>();
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Juicyness(somFloorTom);
        }
    }
    
    private void OnHiHatPerformed(InputAction.CallbackContext context)
    {
        _sprite = _objetoHiHat.GetComponent<SpriteRenderer>();
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Juicyness(somHiHat);
        }
    }
    
    private void OnHiHatPedalPerformed(InputAction.CallbackContext context)
    {
        _sprite = _objetoHiHatPedal.GetComponent<SpriteRenderer>();
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Juicyness(somPedalHiHat);
        }
    }
    
    private void OnCrashPerformed(InputAction.CallbackContext context)
    {
        _sprite = _objetoCrash.GetComponent<SpriteRenderer>();
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Juicyness(somCrash);
        }
    }
    
    private void OnRidePerformed(InputAction.CallbackContext context)
    {
        _sprite = _objetoRide.GetComponent<SpriteRenderer>();
        string teclaPressionada = context.control.name;
        if (context.control.IsPressed())
        {
            double tempoDoToque = context.time;
            Debug.Log($"Ação disparada pela tecla \"{teclaPressionada}\" em {tempoDoToque} segundos");
            Juicyness(somRide);
        }
    }

    private void Juicyness(AudioClip som)
    {
        if (somSnare != null && _sprite != null)
        {
            _audioSource.pitch = Random.Range(pitchMin, pitchMax);
            //_audioSource.PlayOneShot(som);
            _audioSource.clip = som;
            _audioSource.PlayScheduled(AudioSettings.dspTime);
            if (_routine != null)
                StopCoroutine(_routine);
            _routine = StartCoroutine(CorPiscar());
        }
    }

    private IEnumerator CorPiscar()
    {
        _sprite.color = corPiscada;
        yield return new WaitForSeconds(tempoPiscada);
        _sprite.color = corOriginal;
    }
}