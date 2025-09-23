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
    [SerializeField] GameObject TaskArrow;
    [SerializeField] GameObject ChargerArrow;
    public bool tutorialEnabled = false;

    void Awake()
    {
        Instance = this;
    }

    /*public void StartTutorial(bool isWheelMissing)
    {
        if (currentState == StateTutorial.Idle && tutorialEnabled)
        {
            currentState = StateTutorial.Task;
            SwitchState();
        }
    }*/
    public void SwitchState()
    {
        TurnOffArrows();
        switch (currentState)
        {
            case StateTutorial.Charger:
                ChargerArrow.GetComponent<MeshRenderer>().enabled = true;
                break;
            case StateTutorial.Task:
                TaskArrow.GetComponent<MeshRenderer>().enabled = true;
                break;
            default:
                break;
        }
    }

    public void TurnOffArrows()
    {
        ChargerArrow.GetComponent<MeshRenderer>().enabled = false;
        TaskArrow.GetComponent<MeshRenderer>().enabled = false;
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
        TaskArrow = GameObject.Find("SwapBatteryTask(Clone)/Arrow");
        
    }

    
}
