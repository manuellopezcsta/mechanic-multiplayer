using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCounter : BaseCounter
{
    // Codigo de los botones del elevador.
    [SerializeField] ElevatorController elevatorController;
    public override void Interact(Player player)
    {
        elevatorController.ChangeFloorElevator();
        Debug.Log("Piso actual: " + elevatorController.floorNumberElevator);
    }
}