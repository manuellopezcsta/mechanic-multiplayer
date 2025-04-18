using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseMenuScriptUI : MonoBehaviour
{

    [SerializeField] private Button resumeGameButton;
    [SerializeField] private Button restartLevelButton;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Button exitToMainMenuButton;

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
        GameManager.Instance.NukePlayerControllers();
        SceneManager.LoadScene(currentSceneName);
        
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PlayerConfigurationManager.Instance.SelfDestruct();
        SceneManager.LoadScene("Menu");
    }

    private void Awake()
    {
        // Creamos los eventos al momento de interactuar con los botones
        resumeGameButton.onClick.AddListener(ResumeGame);
        restartLevelButton.onClick.AddListener(RestartLevel);
        exitToMainMenuButton.onClick.AddListener(GoToMainMenu);
        // Apagamos la pantalla de pausa al comienzo por las dudas
        pauseScreen.SetActive(false);
    }

    // Funcion que se llama desde el performedAction del INPUT Manager del player para pausar cuando presionen la tecla de pausa.
    public void TogglePause()
    {
        Debug.Log(pauseScreen.activeSelf);
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
