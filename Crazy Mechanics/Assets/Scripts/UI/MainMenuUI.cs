using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {


    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private GameObject creditScreen;
    const string GAME_SCENE_NAME = "Test Demo";


    private void Awake() {
        playButton.onClick.AddListener(() => {
            SceneManager.LoadScene(GAME_SCENE_NAME);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        creditsButton.onClick.AddListener(() => {
            creditScreen.SetActive(true);
        });

        Time.timeScale = 1f;
    }

    void Start()
    {
        creditScreen.SetActive(false);   
    }

}