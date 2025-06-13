using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering; // Se requiere para opciones graficas
using UnityEngine.Rendering.Universal; // Se requiere para URP y sus opciones de sombras

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button deleteDataButton;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Button confirmDeleteButton;
    [SerializeField] private Toggle shadowsToggle;

    private UniversalRenderPipelineAsset urpAsset;
    private float storedShadowDistanceWhenOn = 150f; // Valor estandar para las sombras en caso de que se activen por primera vez.

    // EL CALCULO DE VOLUMEN SE REALIZA USANDO UNA POTENCIA PARA QUE SUENE MEJOR EL CAMBIO AL OIDO HUMANO.

    void Start()
    {
        // Tomamos el valor guardado del volumen y movemos el puntito del slider para que encaje.

        musicSlider.value = SoundManager.Instance.GetMusicVolume();
        sfxSlider.value = SoundManager.Instance.GetSfxVolume();

        // definimos lo que sucede cuando movemos el slider de musica

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);

        // Para el boton de borrar todo
        deleteDataButton.onClick.AddListener(OpenDeleteDataConfirmation);

        // Configuraci�n de sombras para URP
        urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (urpAsset == null)
        {
            Debug.LogError("No se encontro el URP Asset. El boton de sombreado sera deshabilitado.");
            if (shadowsToggle != null)
            {
                shadowsToggle.interactable = false;
            }
        }
        else
        {
            //Debug.Log("URP Asset encontrado: " + urpAsset.name);
            // Si las sombras estan activadas al inicio (distancia > 0), guardamos esa distancia actual.
            // Si estan desactivadas al inicio (distancia == 0), storedShadowDistanceWhenOn mantiene su valor por defecto.
            if (urpAsset.shadowDistance > 0)
            {
                storedShadowDistanceWhenOn = urpAsset.shadowDistance;
            }
            shadowsToggle.isOn = urpAsset.shadowDistance > 0;
            Debug.Log("Distancia inicial de sombras URP: " + urpAsset.shadowDistance + ". Distancia 'activada' guardada: " + storedShadowDistanceWhenOn + ". Toggle configurado en: " + shadowsToggle.isOn);
        }

        // Listener para el interruptor de sombras
        if (shadowsToggle != null && urpAsset != null) // Solo agregar el listener si la configuraci�n es v�lida
        {
            shadowsToggle.onValueChanged.AddListener(ToggleShadows);
            //Debug.Log("Se agrego el listener de sombras.");
        }
    }

    private void ChangeMusicVolume(float newVolume)
    {
        float correctVolume = Mathf.Pow(newVolume, 1.5f);
        SoundManager.Instance.ChangeVolume(correctVolume);
        SoundManager.Instance.PlayButtonClick();
    }

    private void ChangeSfxVolume(float newVolume)
    {
        float correctVolume = Mathf.Pow(newVolume, 1.5f);
        SoundManager.Instance.ChangeVolumeSfx(correctVolume);
        SoundManager.Instance.PlayButtonClick();
    }

    public void ToggleShadows(bool enableShadows) // Si no esta publico no lo veo en el inspector para seleccionar
    {
        if (urpAsset == null)
        {
            Debug.LogError("El URP Asset es nulo, no se pueden activar las sombras.");
            return;
        }

        if (enableShadows)
        {
            // Al activar, restaurar la distancia guardada.
            // Si storedShadowDistanceWhenOn es 0 (por ejemplo, si las sombras estaban desactivadas por defecto y nunca se activaron), seria bueno usar un minimo conocido como 50.
            // Sin embargo, la logica actual usara el valor por defecto 50f si estaban desactivadas inicialmente
            urpAsset.shadowDistance = storedShadowDistanceWhenOn;
            Debug.Log("Intentando activar las sombras. Estableciendo URP shadowDistance en: " + storedShadowDistanceWhenOn);
        }
        else
        {
            // Al desactivar, si las sombras estan activadas, actualizamos storedShadowDistanceWhenOn con la distancia actual.
            if (urpAsset.shadowDistance > 0)
            {
                storedShadowDistanceWhenOn = urpAsset.shadowDistance;
            }
            urpAsset.shadowDistance = 0f;
            Debug.Log("Intentando desactivar las sombras. URP shadowDistance establecido en 0. Distancia 'activada' recordada: " + storedShadowDistanceWhenOn);
        }

        SoundManager.Instance.PlayButtonClick();
    }

    private void OpenDeleteDataConfirmation()
    {
        SoundManager.Instance.PlayButtonClick();
        confirmationPanel.SetActive(true);
        confirmDeleteButton.Select();
    }
}
