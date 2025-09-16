using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManagerWheel : MonoBehaviour
{
    public static TutorialManagerWheel Instance { get; private set; }
    public enum StateTutorial
    {
        Idle,
        FlechaRueda,
        FlechaBalanceo,
        FlechaAuto,
    }
    public StateTutorial currentState;
    [SerializeField] GameObject flechaRueda;
    [SerializeField] GameObject flechaBalanceo;
    [SerializeField] GameObject flechaAuto;
    [SerializeField] BaseCounter targetTask;
    private bool tutorialEnabled = false;


    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        BaseCounter tutorialTask = GameManager.Instance.GetLevelProperties().tutorialTask;
        if (tutorialTask == targetTask)
        {
            tutorialEnabled = true;
        }
    }

    public void StartTutorial(bool isWheelMissing)
    {
        if (currentState == StateTutorial.Idle && tutorialEnabled)
        {
            if (isWheelMissing)
            {
                currentState = StateTutorial.FlechaRueda;
            }
            else
            {
                currentState = StateTutorial.FlechaAuto;
            }

            SwtichState();
        }
    }
    private void SwtichState()
    {
        TurnOffArrows();
        switch (currentState)
        {
            case StateTutorial.FlechaRueda:
                flechaRueda.SetActive(true);
                break;
            case StateTutorial.FlechaBalanceo:
                flechaBalanceo.SetActive(true);
                break;
            case StateTutorial.FlechaAuto:
                flechaAuto.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void TurnOffArrows()
    {
        flechaAuto.SetActive(false);
        flechaBalanceo.SetActive(false);
        flechaRueda.SetActive(false);

    }
    public void BalancedTutorial()
    {
        if (currentState == StateTutorial.FlechaRueda || currentState == StateTutorial.FlechaAuto)
        {
            currentState = StateTutorial.FlechaBalanceo;
            SwtichState();
        }
    }
    public void ReturnWheelTutorial()
    {
        if (currentState == StateTutorial.FlechaBalanceo)
        {
            currentState = StateTutorial.FlechaAuto;
            SwtichState();
        }
    }

    public void CompleteTutorial()
    {
        if (currentState == StateTutorial.FlechaAuto)
        {
            currentState = StateTutorial.Idle;
            SwtichState();
        }
        
    }
}
