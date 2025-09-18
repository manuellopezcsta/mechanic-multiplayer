using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TutorialManagerBattery : MonoBehaviour
{
    public static TutorialManagerBattery Instance { get; private set; }

    public enum StateTutorial
    {
        Idle,
        Charger,
        Task,
    }

    public StateTutorial currentState;
    [SerializeField] GameObject FlechaTask;
    [SerializeField] GameObject FlechaCharger;
    public bool tutorialEnabled = false;

    void Awake()
    {
        Instance = this;
    }

    public void StartTutorial(bool isWheelMissing)
    {
        if (currentState == StateTutorial.Idle && tutorialEnabled)
        {
            currentState = StateTutorial.Task;
            SwitchState();
        }
    }
    public void SwitchState()
    {
        TurnOffArrows();
        switch (currentState)
        {
            case StateTutorial.Charger:
                FlechaCharger.GetComponent<MeshRenderer>().enabled = true;
                break;
            case StateTutorial.Task:
                FlechaCharger.GetComponent<MeshRenderer>().enabled = true;
                break;
            default:
                break;
        }
    }

    public void TurnOffArrows()
    {
        FlechaCharger.GetComponent<MeshRenderer>().enabled = false;
        FlechaTask.GetComponent<MeshRenderer>().enabled = false;
    }

    public void StateChange(StateTutorial inState, StateTutorial outState)
    {
        if (currentState == inState)
        {
            currentState = outState;
            SwitchState();
        }
    }
    public void FindBatteryTaskArrow()
    {
        FlechaTask = GameObject.Find("SwapBatteryTask(Clone)/Arrow");
        
    }

    
}
