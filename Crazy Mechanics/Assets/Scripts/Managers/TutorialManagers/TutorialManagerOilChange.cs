using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManagerOilChange : MonoBehaviour
{
    public static TutorialManagerOilChange Instance { get; private set; }

    public enum StateTutorial
    {
        Idle,
        ElevatorBottom,
        ElevatorFirstFloor,
        Boxes,
        Task,
        Trash,
        OilSpawner
    }

    public StateTutorial currentState;
    [SerializeField] GameObject[] ElevatorArrows;
    [SerializeField] GameObject BoxPileArrow;
    [SerializeField] GameObject Task;
    [SerializeField] GameObject Trash;
    [SerializeField] GameObject OilSpawner;

    void Awake()
    {
        Instance = this;
    }

    public void SwitchState()
    {
        TurnOffArrows();
        switch (currentState)
        {
            case StateTutorial.ElevatorBottom:
                foreach (var obj in ElevatorArrows)
                {
                    obj.GetComponent<MeshRenderer>().enabled = true;
                }
                break;
            case StateTutorial.ElevatorFirstFloor:
                foreach (var obj in ElevatorArrows)
                {
                    obj.GetComponent<MeshRenderer>().enabled = true;
                }
                break;
            case StateTutorial.Boxes:
                BoxPileArrow.GetComponent<MeshRenderer>().enabled = true;
                break;
            case StateTutorial.Task:
                Task.GetComponent<MeshRenderer>().enabled = true;
                break;
            case StateTutorial.Trash:
                Trash.GetComponent<MeshRenderer>().enabled = true;
                break;
            case StateTutorial.OilSpawner:
                OilSpawner.GetComponent<MeshRenderer>().enabled = true;
                break;
            default:
                break;
        }
    }

    public void TurnOffArrows()
    {
        foreach (var obj in ElevatorArrows)
        {
            obj.GetComponent<MeshRenderer>().enabled = false;
        }
        BoxPileArrow.GetComponent<MeshRenderer>().enabled = false;
        Task.GetComponent<MeshRenderer>().enabled = false;
        Trash.GetComponent<MeshRenderer>().enabled = false;
        OilSpawner.GetComponent<MeshRenderer>().enabled = false;
    }

    public void FindOilChangeTaskArrow()
    {

        Task = GameObject.Find("Oil Task(Clone)/Arrow");
        Debug.Log("found task: " + Task.name);

    }
    public void StateChange(StateTutorial inState, StateTutorial outState)
    {
        if (currentState == inState)
        {
            currentState = outState;
            //Caso especial del elevador que va a esperar que llegue el auto
            if (currentState != StateTutorial.ElevatorBottom && currentState != StateTutorial.ElevatorFirstFloor)
            {
                SwitchState();
            }
        }
    }
}
