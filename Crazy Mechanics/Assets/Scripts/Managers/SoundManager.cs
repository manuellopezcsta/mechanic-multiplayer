using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioSource musicSource;

    private float musicVolume = 0.3f;
    private float sfxVolume = 1f;

    // Para los vfx.
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

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
        MotorTool.OnCricketUsed += OnCricketUsed; 
        // Error al usarlo en el menu xq gameManager no existe ahi.
        GameManager.OnCarSpawned += OnCarSpawned;
        TaskOil.OnAddingOil += OnAddingOil;
        TaskOil.OnOilDraining += OnOilDraining;
        LightBoxController.OnFixingLightBox += OnFixingLightBox;
        LightBoxController.OnLightShutdown += OnLightShutdown;
        DisasterManager.OnSpawnedMysteryBox += OnSpawnedMysteryBox;
        MysteryBox.OnOpenedMysteryBox += OnOpenedMysteryBox;
        DisasterManager.OnOilSpillsSpawned += OnOilSPillsSpawned;
        OilSplatter.OnOilSpillCleaning += OnOilSpillCleaning;
        DisasterManager.OnDiscoNight += OnDiscoNight;
        MotorTool.OnDrillUsed += OnDrillUsed;
        TaskDifferential.OnFixingDiff += OnDrillUsed;
    }

    private void OnDrillUsed(object sender, System.EventArgs e)
    {
        MotorTool motorTool = sender as MotorTool;
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.drill, motorTool.transform.position);
    }

    private void OnDiscoNight(object sender, System.EventArgs e)
    {
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.discoNight, transform.position);
    }

    private void OnOilSpillCleaning(object sender, System.EventArgs e)
    {
        OilSplatter oilSpill = sender as OilSplatter;
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.cleaningOil, oilSpill.transform.position);
    }

    private void OnOilSPillsSpawned(object sender, System.EventArgs e)
    {
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.spawnOilSpills, transform.position);
    }

    private void OnOpenedMysteryBox(object sender, System.EventArgs e)
    {
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.mysteryBoxOpen, transform.position);
    }

    private void OnSpawnedMysteryBox(object sender, System.EventArgs e)
    {
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.mysteryBoxSpawn, transform.position);
    }

    private void OnLightShutdown(object sender, System.EventArgs e)
    {
        LightBoxController lightbox = sender as LightBoxController;
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.powerShutdown, lightbox.transform.position);
    }

    private void OnFixingLightBox(object sender, System.EventArgs e)
    {
        LightBoxController lightbox = sender as LightBoxController;
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.lightboxFixing, lightbox.transform.position);
    }

    private void OnOilDraining(object sender, System.EventArgs e)
    {
        TaskOil oilTask = sender as TaskOil;
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.oilDrain, oilTask.transform.position);
    }

    private void OnAddingOil(object sender, System.EventArgs e)
    {
        TaskOil oilTask = sender as TaskOil;
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.oilAdd, oilTask.transform.position);
    }

    private void OnCarSpawned(object sender, System.EventArgs e)
    {
        // Ruido del auto entrando al taller.
        PlaySound(audioClipRefsSO.carEntering, transform.position);
    }

    private void OnCricketUsed(object sender, System.EventArgs e)
    {
        MotorTool motorTool = sender as MotorTool;
        // CAMBIAR X SONIDOS POSTA
        PlaySound(audioClipRefsSO.trash, motorTool.transform.position);
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
        // Money sound
        PlaySound(audioClipRefsSO.delivery, csm.transform.position);
        // Ruido del auto entrando al taller.
        PlaySound(audioClipRefsSO.carExiting, transform.position);
    }

    // Para algun boton
    public void ChangeVolume(float newVolume) {
        // Le cambiamos el volumen al parlante
        musicSource.volume = newVolume;
       
        // Guardamos el valor de volumen en la memoria
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, newVolume);
        PlayerPrefs.Save();
    }

    public void ChangeVolumeSfx(float newVolume)
    {
        // Le cambiamos el volumen al parlante
        sfxVolume = newVolume;

        // Guardamos el valor de volumen en la memoria
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, newVolume);
        PlayerPrefs.Save();
    }

    // Por si lo queremos mostrar en algun lado.
    public float GetMusicVolume() {
        return PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 1);
    }

    public float GetSfxVolume()
    {
        return PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1);
    }

    // Para los sfx
    // Elige uno de los multiples sonidos disponibles y reproduce 1 con el overload.
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
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

    public void PlayEndOfLevelSound() {
        PlaySound(audioClipRefsSO.endOfLevel, transform.position, sfxVolume);
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
        MotorTool.OnCricketUsed -= OnCricketUsed; 
        GameManager.OnCarSpawned -= OnCarSpawned;
        TaskOil.OnAddingOil -= OnAddingOil;
        TaskOil.OnOilDraining -= OnOilDraining;
        LightBoxController.OnFixingLightBox -= OnFixingLightBox;
        LightBoxController.OnLightShutdown -= OnLightShutdown;
        DisasterManager.OnSpawnedMysteryBox -= OnSpawnedMysteryBox;
        MysteryBox.OnOpenedMysteryBox -= OnOpenedMysteryBox;
        DisasterManager.OnOilSpillsSpawned -= OnOilSPillsSpawned;
        OilSplatter.OnOilSpillCleaning -= OnOilSpillCleaning;
        DisasterManager.OnDiscoNight -= OnDiscoNight;
        MotorTool.OnDrillUsed -= OnDrillUsed;
        TaskDifferential.OnFixingDiff -= OnDrillUsed;
    }
}
