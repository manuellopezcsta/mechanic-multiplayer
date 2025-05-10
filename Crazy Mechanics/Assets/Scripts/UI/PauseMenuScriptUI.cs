using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseMenuScriptUI : MonoBehaviour
{

    [SerializeField] private Button resumeGameButton;
    [SerializeField] private Button restartLevelButton;
    [SerializeField] private Button tutorialLevelButton;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Button exitToMainMenuButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TutorialUIManager tutorialUIManager;

    [Header("Settings initial tutorial")]
    [SerializeField] private bool initialTutorial;
    [SerializeField] private Button backTutorialButton;
    [SerializeField] private GameObject initialTutorialPanel;

    //Aca guardaremos los objetos del panel que se apagaran cuando exista un tutorial inicial y se prenderan sino
    [SerializeField] private GameObject[] uiPlayer;


    // EL CALCULO DE VOLUMEN SE REALIZA USANDO UNA POTENCIA PARA QUE SUENE MEJOR EL CAMBIO AL OIDO HUMANO.
    private void ChangeMusicVolume(float newVolume)
    {
        float correctVolume = Mathf.Pow(newVolume, 1.5f);
        SoundManager.Instance.ChangeVolume(correctVolume);
    }

    private void ChangeSfxVolume(float newVolume)
    {
        float correctVolume = Mathf.Pow(newVolume, 1.5f);
        SoundManager.Instance.ChangeVolumeSfx(correctVolume);
    }

    private void Start()
    {

        if(initialTutorial){
            Time.timeScale = 0f;
            changeObjectValue(false);
            initialTutorialPanel.SetActive(true);
            backTutorialButton.Select();

        }else{
            changeObjectValue(true);
            initialTutorialPanel.SetActive(false);
        }

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
        Time.timeScale = 1f; //Ponemos el tiempo a velocidad normal nuevamente
    }

    private void PauseGame()
    {
        pauseScreen.SetActive(true);
        // Seteamos un boton como seleccionado para que funcione con joystick.
        Time.timeScale = 0f;
        resumeGameButton.Select();
    }

    private void RestartLevel()
    {   // Preguntar al game manager el nombre de nivel actual y cargarlo
        Time.timeScale = 1f;       
        string currentSceneName = SceneManager.GetActiveScene().name;
        GameManager.NukePlayerControllers();
        Loader.Load(currentSceneName);        
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PlayerConfigurationManager.Instance.SelfDestruct();
        Loader.Load(Loader.Scene.Menu);
    }
    private void backTutorialStart()
    {
        //Prendemos la interface, desactivamos el panel y damos comienzo al juego
        changeObjectValue(true);
        initialTutorialPanel.SetActive(false);
        Time.timeScale = 1f; //Ponemos el tiempo a velocidad normal nuevamente
    }
    private void changeObjectValue(bool value){
        //Prendemos o apagamos los objetos de la interface que se encuentran agregador en uiPlayer
        foreach(GameObject ui in uiPlayer){
            ui.SetActive(value);
        }
    }
    private void Awake()
    {
        // Creamos los eventos al momento de interactuar con los botones
        resumeGameButton.onClick.AddListener(ResumeGame);
        restartLevelButton.onClick.AddListener(RestartLevel);
        exitToMainMenuButton.onClick.AddListener(GoToMainMenu);
        tutorialLevelButton.onClick.AddListener(tutorialUIManager.ChangeStateTutorialMenu);
        backTutorialButton.onClick.AddListener(backTutorialStart);
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
