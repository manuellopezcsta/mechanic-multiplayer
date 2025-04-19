using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource vfxSource;

    private float musicVolume = 0.3f;

    // Para los vfx.
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    private float sfxVolume = 1f;

    private void Awake() {
        Instance = this;

        musicVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
        musicSource.volume = musicVolume;
        sfxVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    void Start()
    {
        // Nos suscribimos a los eventos de sonindo
        CurrentStationManager.OnCarDelivery += OnCarDelivered;
        Player.OnPickedSomething += OnPickedUpSomething; 
        Player.OnDroppedSomething += OnDroppedSomething;
        TrashCounter.OnAnyObjectTrashed += OnAnyObjectTrashed; 
    }

    private void OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
    }

    private void OnDroppedSomething(object sender, System.EventArgs e)
    {
        Player player = sender as Player;
        PlaySound(audioClipRefsSO.objectDrop, player.transform.position);
    }

    private void OnPickedUpSomething(object sender, System.EventArgs e)
    {
        Player player = sender as Player;
        PlaySound(audioClipRefsSO.objectPickup, player.transform.position);
    }

    private void OnCarDelivered(object sender, System.EventArgs e)
    {
        CurrentStationManager csm = sender as CurrentStationManager;
        PlaySound(audioClipRefsSO.delivery, csm.transform.position);
    }

    // Para algun boton
    public void ChangeVolume() {
        musicVolume += .1f;
        if (musicVolume > 1f) {
            musicVolume = 0f;
        }
        musicSource.volume = musicVolume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, musicVolume);
        PlayerPrefs.Save();
    }

    // Por si lo queremos mostrar en algun lado.
    public float GetMusicVolume() {
        return musicVolume;
    }

    // Para los vfx
    // Elige uno de los multiples sonidos disponibles y reproduce 1 con el overload.
    public void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }

    // Reproduce 1 solo sonido
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * sfxVolume);
    }

    // Para las patitas
    public void PlayFootstepsSound(Vector3 position, float volume) {
        PlaySound(audioClipRefsSO.footstep, position, volume);
    }

    public void PlayObjectDroppedSound(Transform caster)
    {
        PlaySound(audioClipRefsSO.objectDrop, caster.transform.position);
    }

    // Nos desuscribimos de los eventos al destruir el objeto para no generar problemas
    void OnDestroy()
    {
        CurrentStationManager.OnCarDelivery -= OnCarDelivered;
        Player.OnPickedSomething -= OnPickedUpSomething; 
        Player.OnDroppedSomething -= OnDroppedSomething;
        TrashCounter.OnAnyObjectTrashed -= OnAnyObjectTrashed; 
    }
}
