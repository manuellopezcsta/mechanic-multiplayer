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


    private void ChangeMusicVolume(float newVolume)
    {
        SoundManager.Instance.ChangeVolume(newVolume);
    }

    private void ChangeSfxVolume(float newVolume)
    {
        SoundManager.Instance.ChangeVolumeSfx(newVolume);
    }

    private void Start()
    {
        // Tomamos el valor guardado del volumen y movemos el puntito del slider para que encaje

        musicSlider.value = SoundManager.Instance.GetMusicVolume();
        sfxSlider.value = SoundManager.Instance.GetSfxVolume();


        // definimos lo que sucede cuando movemos el slider de musica

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);
    }


    //creamos las funciones para los botones
    private void ResumeGame()
    {
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
        //PlayerConfigurationManager.Instance.SelfDestruct();
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
