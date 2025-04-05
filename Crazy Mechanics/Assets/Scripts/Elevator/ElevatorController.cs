using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Transform[] floors;
    [SerializeField] private float speed;
    public int floorNumberElevator = 0;
    [SerializeField] private bool isMoving;

    public void changeFlorElevator()
    {
        if (!isMoving)
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
            transform.position = Vector3.MoveTowards(transform.position, floors[floorNumberElevator].position, speed * Time.deltaTime);
            if (transform.position == floors[floorNumberElevator].position)
            {
                isMoving = false;
                //Debug.Log("No se mueve mas");
            }
        }
    }
}
