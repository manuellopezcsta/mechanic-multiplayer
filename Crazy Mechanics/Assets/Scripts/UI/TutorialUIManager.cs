
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class TutorialUIManager : MonoBehaviour

{
    [SerializeField] private Button[] tutorialsButtons;
    [SerializeField] private GameObject tutorialScreen;
    [SerializeField] private GameObject tutorialMenu;
    [SerializeField] private TextMeshProUGUI tutorialVideoName;
    private VideoPlayer display;
    [SerializeField] private Button backOptionsButton;



    // Start is called before the first frame update
    void Start()
    {
        //referenciamos el componente imagen del objeto tutorialScreen
        display = tutorialScreen.GetComponent<VideoPlayer>();
        //SetUpTutorialsButtons();
        backOptionsButton.onClick.AddListener(BackOptions);

        // Nos aseguramos que el menu de tutorial este apagado
        tutorialMenu.SetActive(false);
        tutorialsButtons[0].Select();
        //tutorialsButtons[0].Select();
    }

    private void BackOptions()
    {
        tutorialMenu.SetActive(false);
        // Remarcamos el boton de Resume
        GameObject.Find("Canvas").GetComponent<PauseMenuScriptUI>().GetResumeButton().Select();
    }

    public void ChangeVideo(VideoClip video, string videoName)
    {
        //creamos la funcion para cambiar los sprites
        display.Stop();
        display.clip = video;
        display.Play();
        tutorialVideoName.text = videoName;
    }

    /*private void SetUpTutorialsButtons()
    {
        //Buscamos el valor en la lista de botones y lo replicamos en la lista de sprites
        //para que cambie segun cual de los botones estamos utilizando
        for(int i = 0; i < tutorialsButtons.Length; i++) {
            int buttonIndex = i;
            //tutorialsButtons[i].onClick.AddListener(() => ChangeVideo());
            //Debug.Log(i);
        }
    }*/

    public void ChangeStateTutorialMenu()
    {
        SoundManager.Instance.PlayButtonClick();
        if (tutorialMenu.activeSelf)
        {
            tutorialMenu.SetActive(false);
        }
        else
        {
            tutorialMenu.SetActive(true);
            tutorialsButtons[0].Select();
        }
    }

}
