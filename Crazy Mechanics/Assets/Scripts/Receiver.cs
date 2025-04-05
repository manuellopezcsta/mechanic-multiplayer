using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : BaseCounter
{
    [SerializeField] private ObjectsSO objectsSO;
    [SerializeField] private bool taskComplete;
    [SerializeField] private float timeRequest;

    public override void Interact(Player player)
    {

        // Logica para dejar objetos
        if (!HasCarObject()) {
            // There is no obj here and check if they are the same object
            if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == objectsSO) {
                // El player tiene algo en la mano
                taskComplete = false;
                player.GetCarObject().SetCarObjectParent(this);
                StartCoroutine(TimeToRequest(timeRequest));
            } else {
                // Player no tiene nada en la mano

            }
        } else {
            // There is a car obj here already.
            if(player.HasCarObject() && taskComplete) {
                // Player is carrying something
            } else {
                // Player is not carrying anything. He only takes it when he finishes the task.
                if(taskComplete){
                GetCarObject().SetCarObjectParent(player);
                }
            }
        }
    }

    IEnumerator TimeToRequest(float timeRequest){
        yield return new WaitForSeconds(timeRequest);
        taskComplete = true;
    }
}
