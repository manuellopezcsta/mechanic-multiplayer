using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // creamos las funciones para cambio de volumen en sfx y musica

    private void ChangeMusicVolume(float newVolume)
    {
        //llamamos al sound manager para que efectue el cambio de volumen
        SoundManager.Instance.ChangeVolume(newVolume);
    }

    private void ChangeSfxVolume(float newVolume)
    {
        //llamamos al sound manager para que efectue el cambio de volumen
        SoundManager.Instance.ChangeVolumeSfx(newVolume);
    }

    
    void Start()
    {
        // Tomamos el valor guardado del volumen y movemos el puntito del slider para que encaje.

        musicSlider.value = SoundManager.Instance.GetMusicVolume();


        sfxSlider.value = SoundManager.Instance.GetSfxVolume();


        // definimos lo que sucede cuando movemos el slider de musica

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
