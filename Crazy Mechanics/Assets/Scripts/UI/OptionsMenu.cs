using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // EL CALCULO DE VOLUMEN SE REALIZA USANDO UNA POTENCIA PARA QUE SUENE MEJOR EL CAMBIO AL OIDO HUMANO.

    void Start()
    {
        // Tomamos el valor guardado del volumen y movemos el puntito del slider para que encaje.

        musicSlider.value = SoundManager.Instance.GetMusicVolume();
        sfxSlider.value = SoundManager.Instance.GetSfxVolume();

        // definimos lo que sucede cuando movemos el slider de musica

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);
    }

    // creamos las funciones para cambio de volumen en sfx y musica
    private void ChangeMusicVolume(float newVolume)
    {
        //llamamos al sound manager para que efectue el cambio de volumen
        float correctVolume = Mathf.Pow(newVolume, 1.5f);
        SoundManager.Instance.ChangeVolume(correctVolume);
    }

    private void ChangeSfxVolume(float newVolume)
    {
        //llamamos al sound manager para que efectue el cambio de volumen
        float correctVolume = Mathf.Pow(newVolume, 1.5f);
        SoundManager.Instance.ChangeVolumeSfx(correctVolume);
    }
}
