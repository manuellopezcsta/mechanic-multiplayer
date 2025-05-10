using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InitialTutorialUI : MonoBehaviour
{
    [Header("Settings Initial tutorial")]
    [SerializeField] private Button backTutorialButton;
    [SerializeField] private GameObject initialTutorialPanel;

    //Aca guardaremos los objetos del panel que se apagaran cuando exista un tutorial inicial y se prenderan sino
    [SerializeField] private GameObject[] uiPlayer;

    void Awake()
    {
        backTutorialButton.onClick.AddListener(ExitStartTutorialScreen);
    }

    void Start()
    {
        LevelProperties levelProperties = GameManager.Instance.GetLevelProperties();
        // Checkeamos para ver si hay un tutorial al comienzo del nivel
        if (levelProperties.hasTutorial)
        {
            Time.timeScale = 0f;
            // Seteamos la imagen
            initialTutorialPanel.GetComponent<Image>().sprite = levelProperties.tutorialImage;
            // Apagamos los elementos de la UI
            SwitchUIElements(false);
            // Mostramos
            initialTutorialPanel.SetActive(true);
            backTutorialButton.Select();
        }
        else
        {
            SwitchUIElements(true);
            initialTutorialPanel.SetActive(false);
        }
    }

        private void ExitStartTutorialScreen()
    {
        //Prendemos la interface, desactivamos el panel y damos comienzo al juego
        SwitchUIElements(true);
        initialTutorialPanel.SetActive(false);
        Time.timeScale = 1f; //Ponemos el tiempo a velocidad normal nuevamente
    }

    private void SwitchUIElements(bool value){
        //Prendemos o apagamos los objetos de la interface que se encuentran agregador en uiPlayer
        foreach(GameObject ui in uiPlayer){
            ui.SetActive(value);
        }
    }

}
