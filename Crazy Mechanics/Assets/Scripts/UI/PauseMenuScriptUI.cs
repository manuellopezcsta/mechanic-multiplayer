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

    private void RestartLevel()
    {   // Preguntar al game manager el nombre de nivel actual y cargarlo
        // Time.timeScale = 1f;       
        // SceneManager.LoadScene("nivel actual");
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(GameObject.Find("PlayerConfigurationManager"));
        SceneManager.LoadScene("Menu");
    }

    private void Awake()
    {
        // Creamos los eventos al momento de interactuar con los botones
        resumeGameButton.onClick.AddListener(ResumeGame);
        restartLevelButton.onClick.AddListener(RestartLevel);
        exitToMainMenuButton.onClick.AddListener(GoToMainMenu);
    }


    // Update is called once per frame
    void Update()
    {
        // chequeamos al precionar la tecla escape si el menu esta cerrado, si es asi, lo abre, sino lo cierra
        // pausamos el tiempo en caso de abrir el menu de pausa, en caso de cerrarlo lo reactivamos.
        if (Input.GetKeyDown("escape"))
        {
            if (pauseScreen.activeSelf == false)
            {
                pauseScreen.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                pauseScreen.SetActive(false);
                Time.timeScale = 1f;
            }

        }
    }
}
