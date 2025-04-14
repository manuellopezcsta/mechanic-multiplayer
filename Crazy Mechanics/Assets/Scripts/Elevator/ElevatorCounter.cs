using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCounter : BaseCounter
{

    [SerializeField] ElevatorController elevatorController;
    public override void Interact(Player player)
    {
        elevatorController.ChangeFlorElevator();
        Debug.Log("Piso actual: " + elevatorController.floorNumberElevator);
    }
}