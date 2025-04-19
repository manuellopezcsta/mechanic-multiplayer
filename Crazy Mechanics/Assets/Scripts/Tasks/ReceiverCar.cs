using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiverCar : BaseCounter
{
    [SerializeField] private ObjectsSO objectsSO;
    [SerializeField] private bool taskComplete;

    public override void Interact(Player player)
    {

        // Logica para dejar objetos
        if (!HasCarObject() && !taskComplete) {
            // There is no obj here and check if they are the same object
            if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == objectsSO) {
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
                taskComplete = true;
            }
        }
    }
}