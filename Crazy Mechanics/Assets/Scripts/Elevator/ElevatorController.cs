using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Transform[] floors;
    [SerializeField] private float speed;
    public int floorNumberElevator = 0;
    [SerializeField] private bool isMoving;
    [SerializeField] Transform elevatorArms;
    [SerializeField] private int nextFloor;
    private bool inTutorial = false;
    CurrentStationManager csm;

    public event EventHandler<OnMovingChangedEventArgs> OnMovingChanged;
    public class OnMovingChangedEventArgs : EventArgs
    {
        public bool isMoving;
    }

    void Start()
    {
        csm = GetComponent<CurrentStationManager>();
        inTutorial = TutorialManagerOilChange.Instance != null;
    }
    public void ChangeFloorElevator()
    {
        if (!isMoving && !csm.IsElevatorLocked() && GameManager.Instance.IsPowerEnabled() && !csm.isFree() && !csm.currentCar.canMove)
        {
            switch (floorNumberElevator)
            {
                case 0:
                    nextFloor = 1;
                    csm.currentCar.bottomCar.enabled = false;
                    break;
                case 1:
                    nextFloor = 2;
                    csm.currentCar.bottomCar.enabled = false;
                    break;
                case 2:
                    nextFloor = 0;
                    csm.currentCar.bottomCar.enabled = true;
                    break;
            }
            TutorialUpdate();
            isMoving = true;
            Debug.Log("update tutorial");
            OnMovingChanged?.Invoke(this, new OnMovingChangedEventArgs
            {
                isMoving = this.isMoving
            });
        }
    }

    void Update()
    {
        if (isMoving)
        {
            elevatorArms.position = Vector3.MoveTowards(elevatorArms.position, floors[nextFloor].position, speed * Time.deltaTime);
            if (elevatorArms.position == floors[nextFloor].position)
            {
                isMoving = false;

                floorNumberElevator = nextFloor;
                //Debug.Log("No se mueve mas");

                OnMovingChanged?.Invoke(this, new OnMovingChangedEventArgs
                {
                    isMoving = this.isMoving
                });
            }
        }
    }
    public bool CheckIfElevatorIsMoving()
    {
        return isMoving;
    }
    //Maneja los cmabios de las flechas de tutorial dependiendo del estado
    private void TutorialUpdate()
    {
        //si no esta en tutorial sale inmediatamente
        if (!inTutorial)
        {
            return;
        }
        
        Debug.Log("Tutorial elevator state "+ TutorialManagerOilChange.Instance.currentState.ToString());
        switch (TutorialManagerOilChange.Instance.currentState)
        {
            case TutorialManagerOilChange.StateTutorial.ElevatorFirstFloor:
                if (nextFloor == 1)
                {
                    TutorialManagerOilChange.Instance.StateChange(TutorialManagerOilChange.StateTutorial.ElevatorFirstFloor, TutorialManagerOilChange.StateTutorial.Boxes);
                }
                break;
            case TutorialManagerOilChange.StateTutorial.ElevatorBottom:
                if (nextFloor == 0)
                {
                    TutorialManagerOilChange.Instance.StateChange(TutorialManagerOilChange.StateTutorial.ElevatorBottom, TutorialManagerOilChange.StateTutorial.OilSpawner);
                }
                break;
            case TutorialManagerOilChange.StateTutorial.Boxes:
                if (nextFloor != 1)
                {
                    TutorialManagerOilChange.Instance.StateChange(TutorialManagerOilChange.StateTutorial.Boxes, TutorialManagerOilChange.StateTutorial.ElevatorFirstFloor);
                }
                break;
            case TutorialManagerOilChange.StateTutorial.OilSpawner:
                if (nextFloor != 0)
                {
                    TutorialManagerOilChange.Instance.StateChange(TutorialManagerOilChange.StateTutorial.OilSpawner, TutorialManagerOilChange.StateTutorial.ElevatorBottom);
                }
                break;
            default:
                break;
        }
    }
}
