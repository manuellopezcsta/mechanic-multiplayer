using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalacingTool : BaseCounter
{
    [SerializeField] private ObjectsSO wheel;

    [SerializeField] private ObjectsSO balancedWheel;
    [SerializeField] private bool taskComplete;
    [SerializeField] private float timeRequest;

    

    public override void Interact(Player player)
    {
        Debug.Log("Se entro aca");
        // Logica para dejar objetos
        if (!HasCarObject()) {
            // There is no obj here and check if they are the same object
            if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == wheel && GameManager.Instance.IsPowerEnabled()) {
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
                StartCoroutine(TimeToRequest(timeRequest));
            } else {
                // Player no tiene nada en la mano

            }
        } else {
            // There is a car obj here already.
            if(!player.HasCarObject() && taskComplete) {
                GetCarObject().SetCarObjectParent(player);
                taskComplete = false;
            } 
        }
    }

    IEnumerator TimeToRequest(float timeRequest){
        yield return new WaitForSeconds(timeRequest);
        taskComplete = true;
        Destroy(GetCarObject().gameObject);
        ClearCarObject();
        Transform outputSOPreFab = Instantiate(balancedWheel.prefab, GetCarObjectFollowTransform());
        outputSOPreFab.GetComponent<CarObject>().SetCarObjectParent(this);
    }
}
