using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InitialTutorialUI : MonoBehaviour
{
    [Header("Settings Initial tutorial")]
    [SerializeField] private GameObject initialTutorialPanel;

    //Aca guardaremos los objetos del panel que se apagaran cuando exista un tutorial inicial y se prenderan sino
    [SerializeField] private GameObject[] uiPlayer;
    private int currentScreenTutorialNumber;
    private LevelProperties levelProperties;
    [SerializeField] private Button[] buttons; //0 = Previous, 1 = Next;
    [SerializeField] private TextMeshProUGUI buttonNext;

    void Awake()
    {
        buttons[0].onClick.AddListener(PreviousTutorialScreen);
        buttons[0].gameObject.SetActive(false);
        buttons[1].onClick.AddListener(NextTutorialScreen);

    }

    void Start()
    {
        // Checkeamos para ver si hay un tutorial al comienzo del nivel
        levelProperties = GameManager.Instance.GetLevelProperties();
        if (levelProperties.hasTutorial)
        {
            Time.timeScale = 0f;
            // Seteamos la imagen
            initialTutorialPanel.GetComponent<Image>().sprite = levelProperties.tutorialImage[0];
            currentScreenTutorialNumber = 0;
            // Apagamos los elementos de la UI
            SwitchUIElements(false);
            // Mostramos
            initialTutorialPanel.SetActive(true);
            StartCoroutine(SelectButtonNextFrame());
        }
        else
        {
            SwitchUIElements(true);
            initialTutorialPanel.SetActive(false);
        }
    }

    private IEnumerator SelectButtonNextFrame()
    {
        yield return null; // Waits for the end of the current frame
        buttons[1].Select();
    }


    private void PreviousTutorialScreen()
    {
        if (currentScreenTutorialNumber == 0)
        {
            return;
        }
        else
        {
            buttonNext.text = "NEXT";
            currentScreenTutorialNumber -= 1;
            initialTutorialPanel.GetComponent<Image>().sprite = levelProperties.tutorialImage[currentScreenTutorialNumber];
            if (currentScreenTutorialNumber == 0)
            {
                buttons[0].gameObject.SetActive(false);
                buttons[1].Select();
            }
        }

    }
    private void NextTutorialScreen()
    {
        if (currentScreenTutorialNumber == levelProperties.tutorialImage.Length - 2)
        {
            buttonNext.text = "EXIT";
            currentScreenTutorialNumber += 1;
            initialTutorialPanel.GetComponent<Image>().sprite = levelProperties.tutorialImage[currentScreenTutorialNumber];
            buttons[0].gameObject.SetActive(true);
        }
        else if (currentScreenTutorialNumber == levelProperties.tutorialImage.Length - 1)
        {
            ExitStartTutorialScreen();
        }
        else
        {
            currentScreenTutorialNumber += 1;
            initialTutorialPanel.GetComponent<Image>().sprite = levelProperties.tutorialImage[currentScreenTutorialNumber];
            buttons[0].gameObject.SetActive(true);
        }
    }

    private void ExitStartTutorialScreen()
    {
        //Prendemos la interface, desactivamos el panel y damos comienzo al juego
        SwitchUIElements(true);
        initialTutorialPanel.SetActive(false);
        Time.timeScale = 1f; //Ponemos el tiempo a velocidad normal nuevamente
    }

    private void SwitchUIElements(bool value)
    {
        //Prendemos o apagamos los objetos de la interface que se encuentran agregador en uiPlayer
        foreach (GameObject ui in uiPlayer)
        {
            ui.SetActive(value);
        }
    }

}
