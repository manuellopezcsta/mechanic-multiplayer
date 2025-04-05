using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCounter : BaseCounter
{

    [SerializeField] ElevatorController elevatorController;
    [SerializeField] private ObjectsSO objectsSO;
    public override void Interact(Player player)
    {
        elevatorController.changeFlorElevator();
        Debug.Log("Piso actual: " + elevatorController.floorNumberElevator);

    }
}