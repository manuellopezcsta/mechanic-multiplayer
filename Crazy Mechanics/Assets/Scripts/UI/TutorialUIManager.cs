using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    [SerializeField] private Button[] tutorialsButtons;
    [SerializeField] private Sprite[] tutorialSprites;
    [SerializeField] private GameObject tutorialScreen;
    [SerializeField] private GameObject tutorialMenu;
    private Image display;
    [SerializeField] private Button backOptionsButton;



    // Start is called before the first frame update
    void Start()
    {
        //referenciamos el componente imagen del objeto tutorialScreen
        display = tutorialScreen.GetComponent<Image>();
        SetUpTutorialsButtons();
        backOptionsButton.onClick.AddListener(BackOptions);

        // Nos aseguramos que el menu de tutorial este apagado
        tutorialMenu.SetActive(false);
        tutorialsButtons[0].Select();
    }

    private void BackOptions()
    {
        tutorialMenu.SetActive(false);
    }

    private void ChangeSprite(int x)
    {
        //creamos la funcion para cambiar los sprites
        display.sprite = tutorialSprites[x];
    }

    private void SetUpTutorialsButtons()
    {
        //Buscamos el valor en la lista de botones y lo replicamos en la lista de sprites
        //para que cambie segun cual de los botones estamos utilizando
        for(int i = 0; i < tutorialsButtons.Length; i++) {
            int buttonIndex = i;
            tutorialsButtons[i].onClick.AddListener(() => ChangeSprite(buttonIndex));
            //Debug.Log(i);
        }
    }

    public void ChangeStateTutorialMenu()
    {
        if (tutorialMenu.activeSelf)
        {
            tutorialMenu.SetActive(false);
        } else
        {
            tutorialMenu.SetActive(true);
        }
    } 

}
