using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanelUI : MonoBehaviour
{
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Button clearDataButton;

    void Awake()
    {
        yesButton.onClick.AddListener(NukePlayerData);
        noButton.onClick.AddListener(ExitConfirmationPanel);
    }

    // Funcion Donde nukeamos todo los save Files
    private void NukePlayerData()
    {
        PlayerPrefs.DeleteAll();
        ExitConfirmationPanel();
    }

    private void ExitConfirmationPanel()
    {
        gameObject.SetActive(false);
        clearDataButton.Select();
    }
}
