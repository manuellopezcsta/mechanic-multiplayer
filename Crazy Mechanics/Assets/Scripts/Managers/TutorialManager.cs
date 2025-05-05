using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Button[] tutorialsButtons;
    [SerializeField] private Button returnButton;
    [SerializeField] private Sprite[] tutorialsSprites;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Image display;


    void Start()
    {
        SetupTutorialButtons();
        returnButton.onClick.AddListener(SwitchStateTutorialMenu);
    }
    private void ChangeSprite(int i)
    {
        //Seleccionamos la imagen correspondiente al tuto
        display.sprite = tutorialsSprites[i];
    }

    private void SetupTutorialButtons()
    {
        for (int i = 0; i < tutorialsButtons.Length; i++)
        {
            //guardamos la variable en un int local ya que el changeSprite no funcionaba.
            int buttonIndex = i; 
            //utilizamos lambda para separar los changeSprite 
            tutorialsButtons[i].onClick.AddListener(() => ChangeSprite(buttonIndex));
        }
    }

    public void SwitchStateTutorialMenu()
    {
        if(tutorialPanel.activeSelf){
        tutorialPanel.SetActive(false);
        }else{
            tutorialPanel.SetActive(true);
            tutorialsButtons[0].Select();
        }
    }

}
