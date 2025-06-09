using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private GameObject creditScreen;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private Button goBackCreditsButton;
    [SerializeField] private Button goBackOptionsButton;
    [SerializeField] private Button goBackControlsButton;
    [SerializeField] private GameObject optionsScreen;
    const string GAME_SCENE_NAME = "CharacterSelect";

    // Creamos las funciones para los botones
    private void StartGame() {
        Loader.Load(Loader.Scene.CharacterSelect);
    }

    private void QuitGame() {
        Application.Quit();
    }

    private void OpenCredits() {
        creditScreen.SetActive(true);
        goBackCreditsButton.Select();
    }

    private void ExitCredits() {
        creditScreen.SetActive(false);
        playButton.Select();
    }

    private void OpenOptions(){
        optionsScreen.SetActive(true);
        goBackOptionsButton.Select();
    }

    private void ExitOptions(){
        optionsScreen.SetActive(false);
        playButton.Select();
    }
    
    private void OpenControls(){
        controlsScreen.SetActive(true);
        goBackControlsButton.Select();
    }

    private void ExitControls(){
        controlsScreen.SetActive(false);
        playButton.Select();
    }


    private void Awake()
    {
        //Agregamos los eventos que queremos que pasen al hacer click en el boton

        playButton.onClick.AddListener(StartGame);

        quitButton.onClick.AddListener(QuitGame);

        creditsButton.onClick.AddListener(OpenCredits);

        optionsButton.onClick.AddListener(OpenOptions);

        controlsButton.onClick.AddListener(OpenControls);

        goBackCreditsButton.onClick.AddListener(ExitCredits);

        goBackOptionsButton.onClick.AddListener(ExitOptions);

        goBackControlsButton.onClick.AddListener(ExitControls);

        Time.timeScale = 1f;
    }

    void Start()
    {
       // Nos aseguramos de no mostrar la pantalla de creditos antes de hacer click en el boton
        creditScreen.SetActive(false);  
        playButton.Select();
    }

}