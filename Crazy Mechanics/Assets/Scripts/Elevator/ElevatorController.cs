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

    public void ChangeFlorElevator()
    {
        CurrentStationManager csm = GetComponent<CurrentStationManager>();

        if (!isMoving && !csm.IsElevatorLocked() && GameManager.Instance.IsPowerEnabled() && !csm.isFree())
        {
            switch (floorNumberElevator)
            {
                case 0:
                    floorNumberElevator = 1;
                    break;
                case 1:
                    floorNumberElevator = 2;
                    break;
                case 2:
                    floorNumberElevator = 0;
                    break;
            }
            isMoving = true;
            //Debug.Log("Se empieza a mover");
        }
    }
    void Update()
    {
        if (isMoving)
        {
            elevatorArms.position = Vector3.MoveTowards(elevatorArms.position, floors[floorNumberElevator].position, speed * Time.deltaTime);
            if (elevatorArms.position == floors[floorNumberElevator].position)
            {
                isMoving = false;
                //Debug.Log("No se mueve mas");
            }
        }
    }
}
