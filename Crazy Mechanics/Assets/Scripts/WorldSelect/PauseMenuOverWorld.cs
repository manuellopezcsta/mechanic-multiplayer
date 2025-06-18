using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuOverWorld : MonoBehaviour
{

    [SerializeField] private Button resumeGameButton;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Button exitToMainMenuButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    

    // EL CALCULO DE VOLUMEN SE REALIZA USANDO UNA POTENCIA PARA QUE SUENE MEJOR EL CAMBIO AL OIDO HUMANO.
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

    private void Start()
    {
        // Tomamos el valor guardado del volumen y movemos el puntito del slider para que encaje

        musicSlider.value = SoundManager.Instance.GetMusicVolume();
        sfxSlider.value = SoundManager.Instance.GetSfxVolume();


        // definimos lo que sucede cuando movemos el slider de musica

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);

        // Intento x arreglar el flow cuando se va de menu a overworld y a el lv. 12/05
        PlayerConfigurationManager.Instance.SwitchInputMethod(true);
    }


    //creamos las funciones para los botones
    private void ResumeGame()
    {
        SoundManager.Instance.PlayButtonClick();
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;//Resume time
    }

    private void PauseGame()
    {
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;//Stop time
        // Seteamos un boton como seleccionado para que funcione con joystick.
        resumeGameButton.Select();
    }

  

    private void GoToMainMenu()
    {
        SoundManager.Instance.PlayButtonClick();
        //PlayerConfigurationManager.Instance.SelfDestruct();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Destroy(player); // Se destruye el player aqui xq sino, el Player Conf manager se destruye antes y el player pierde la referencia x lo cual no se 
        // Desuscribe de los eventos , y esto causa que al volver a entrar al juego x alguna razon retome los eventos que no se desuscribieron y se rompa.
        Destroy(PlayerConfigurationManager.Instance.gameObject);
        Loader.Load(Loader.Scene.Menu);
    }

    private void Awake()
    {
        // Creamos los eventos al momento de interactuar con los botones
        resumeGameButton.onClick.AddListener(ResumeGame);
        exitToMainMenuButton.onClick.AddListener(GoToMainMenu);
        // Apagamos la pantalla de pausa al comienzo por las dudas
        pauseScreen.SetActive(false);
    }

    // Funcion que se llama desde el performedAction del INPUT Manager del player para pausar cuando presionen la tecla de pausa.
    public void TogglePause()
    {
        SoundManager.Instance.PlayButtonClick();
        //Debug.Log(pauseScreen.activeSelf);
        if (pauseScreen.activeSelf == false)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
}
