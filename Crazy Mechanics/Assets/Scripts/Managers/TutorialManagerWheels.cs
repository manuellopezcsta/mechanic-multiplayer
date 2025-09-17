using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class TutorialManagerWheel : MonoBehaviour
{
    public static TutorialManagerWheel Instance { get; private set; }
    public enum StateTutorial
    {
        Idle,
        FlechaPilaRueda,
        FlechaBalanceo,
        FlechaAuto,
    }
    public StateTutorial currentState;
    [SerializeField] GameObject flechaPilaRueda;
    [SerializeField] GameObject flechaBalanceo;
    [SerializeField] GameObject flechaAuto;
    [SerializeField] BaseCounter targetTask;
    public bool tutorialEnabled = false;


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
                currentState = StateTutorial.FlechaPilaRueda;
            }
            else
            {
                currentState = StateTutorial.FlechaAuto;
            }

            SwitchState();
        }
    }
    private void SwitchState()
    {
        TurnOffArrows();
        switch (currentState)
        {
            case StateTutorial.FlechaPilaRueda:
                //flechaPilaRueda.SetActive(true);
                flechaPilaRueda.GetComponent<MeshRenderer>().enabled = true;
                break;
            case StateTutorial.FlechaBalanceo:
                //flechaBalanceo.SetActive(true);
                flechaBalanceo.GetComponent<MeshRenderer>().enabled = true;
                break;
            case StateTutorial.FlechaAuto:
                //flechaAuto.SetActive(true);
                flechaAuto.GetComponent<MeshRenderer>().enabled = true;
                break;
            default:
                break;
        }
    }

    private void TurnOffArrows()
    {
        /*flechaAuto.SetActive(false);
        flechaBalanceo.SetActive(false);
        flechaPilaRueda.SetActive(false);*/
        flechaPilaRueda.GetComponent<MeshRenderer>().enabled = false;
        flechaBalanceo.GetComponent<MeshRenderer>().enabled = false;
        flechaAuto.GetComponent<MeshRenderer>().enabled = false;

    }
    public void BalancedTutorial()
    {
        if (currentState == StateTutorial.FlechaPilaRueda || currentState == StateTutorial.FlechaAuto)
        {
            currentState = StateTutorial.FlechaBalanceo;
            SwitchState();
        }
    }
    public void ReturnWheelTutorial()
    {
        if (currentState == StateTutorial.FlechaBalanceo)
        {
            currentState = StateTutorial.FlechaAuto;
            SwitchState();
        }
    }

    public void CompleteTutorial()
    {
        if (currentState == StateTutorial.FlechaAuto)
        {
            currentState = StateTutorial.Idle;
            SwitchState();
        }

    }
    public void FindWheelTaskArrow()
    {
        flechaAuto = GameObject.Find("WheelTask(Clone)/Arrow");
    }
    public void StateChange(StateTutorial inState, StateTutorial outState){
        if (currentState == inState) {
            currentState = outState;
            SwitchState();
        }
    }
}