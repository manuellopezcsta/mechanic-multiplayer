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

    public event EventHandler<OnMovingChangedEventArgs> OnMovingChanged;
    public class OnMovingChangedEventArgs : EventArgs
    {
        public bool isMoving;
    }
    public void ChangeFloorElevator()
    {
        CurrentStationManager csm = GetComponent<CurrentStationManager>();

        if (!isMoving && !csm.IsElevatorLocked() && GameManager.Instance.IsPowerEnabled() && !csm.isFree() && !csm.currentCar.canMove)
        {
            Debug.Log("Piso actual" + floorNumberElevator);
            switch (floorNumberElevator)
            {
                case 0:
                    nextFloor = 1;
                    csm.currentCar.bottomCar.enabled = false;
                    Debug.Log("Piso temporal" + nextFloor);
                    break;
                case 1:
                    nextFloor = 2;
                    csm.currentCar.bottomCar.enabled = false;
                    Debug.Log("Piso temporal" + nextFloor);
                    break;
                case 2:
                    nextFloor = 0;
                    csm.currentCar.bottomCar.enabled = true;
                    Debug.Log("Piso temporal" + nextFloor);
                    break;
            }
            isMoving = true;
            //Debug.Log("Se empieza a mover");
            OnMovingChanged?.Invoke(this, new OnMovingChangedEventArgs {
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

                OnMovingChanged?.Invoke(this, new OnMovingChangedEventArgs {
                isMoving = this.isMoving
            });
            }
        }
    }
    public bool CheckIfElevatorIsMoving(){
        return isMoving;
    }
}
