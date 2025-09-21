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
        WheelPileArrow,
        BalanceToolArrow,
        TaskArrow,
    }
    public StateTutorial currentState;
    [SerializeField] GameObject WheelPileArrow;
    [SerializeField] GameObject BalanceToolArrow;
    [SerializeField] GameObject TaskArrow;
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
                currentState = StateTutorial.WheelPileArrow;
            }
            else
            {
                currentState = StateTutorial.TaskArrow;
            }

            SwitchState();
        }
    }
    private void SwitchState()
    {
        TurnOffArrows();
        switch (currentState)
        {
            case StateTutorial.WheelPileArrow:
                //WheelPileArrow.SetActive(true);
                WheelPileArrow.GetComponent<MeshRenderer>().enabled = true;
                break;
            case StateTutorial.BalanceToolArrow:
                //BalanceToolArrow.SetActive(true);
                BalanceToolArrow.GetComponent<MeshRenderer>().enabled = true;
                break;
            case StateTutorial.TaskArrow:
                //TaskArrow.SetActive(true);
                TaskArrow.GetComponent<MeshRenderer>().enabled = true;
                break;
            default:
                break;
        }
    }

    private void TurnOffArrows()
    {
        /*TaskArrow.SetActive(false);
        BalanceToolArrow.SetActive(false);
        WheelPileArrow.SetActive(false);*/
        WheelPileArrow.GetComponent<MeshRenderer>().enabled = false;
        BalanceToolArrow.GetComponent<MeshRenderer>().enabled = false;
        TaskArrow.GetComponent<MeshRenderer>().enabled = false;

    }
    public void BalancedTutorial()
    {
        if (currentState == StateTutorial.WheelPileArrow || currentState == StateTutorial.TaskArrow)
        {
            currentState = StateTutorial.BalanceToolArrow;
            SwitchState();
        }
    }
    public void ReturnWheelTutorial()
    {
        if (currentState == StateTutorial.BalanceToolArrow)
        {
            currentState = StateTutorial.TaskArrow;
            SwitchState();
        }
    }

    public void CompleteTutorial()
    {
        if (currentState == StateTutorial.TaskArrow)
        {
            currentState = StateTutorial.Idle;
            SwitchState();
        }

    }
    public void FindWheelTaskArrow()
    {
        TaskArrow = GameObject.Find("WheelTask(Clone)/Arrow");
    }
    public void StateChange(StateTutorial inState, StateTutorial outState){
        if (currentState == inState) {
            currentState = outState;
            SwitchState();
        }
    }
}