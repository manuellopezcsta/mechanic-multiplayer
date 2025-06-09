using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button deleteDataButton;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Button confirmDeleteButton;
    [SerializeField] private Toggle shadowsToggle;

    // EL CALCULO DE VOLUMEN SE REALIZA USANDO UNA POTENCIA PARA QUE SUENE MEJOR EL CAMBIO AL OIDO HUMANO.

    void Start()
    {
        // Tomamos el valor guardado del volumen y movemos el puntito del slider para que encaje.

        musicSlider.value = SoundManager.Instance.GetMusicVolume();
        sfxSlider.value = SoundManager.Instance.GetSfxVolume();
        shadowsToggle.isOn = QualitySettings.shadows == ShadowQuality.All;

        // definimos lo que sucede cuando movemos el slider de musica

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);

        // Para las shadows
        shadowsToggle.onValueChanged.AddListener(ToggleShadows);

        // Para el boton de borrar todo
        deleteDataButton.onClick.AddListener(OpenDeleteDataConfirmation);
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

    private void ToggleShadows(bool enableShadows)
    {
        QualitySettings.shadows = enableShadows ? ShadowQuality.All : ShadowQuality.Disable;
        Debug.Log("Shadows set to: " + enableShadows);
    }

    private void OpenDeleteDataConfirmation()
    {
        confirmationPanel.SetActive(true);
        confirmDeleteButton.Select();
    }
}
