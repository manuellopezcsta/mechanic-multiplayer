using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TaskOil : BaseCounter
{
    public CarController carController;
    [SerializeField] private ObjectsSO box;
    [SerializeField] private ObjectsSO oil;
    [SerializeField] private ObjectsSO boxFull;
    [SerializeField] private bool taskComplete;
    [SerializeField] private float timeRequest;

    [SerializeField] private bool itHasDirtyOil = true;


    public override void Interact(Player player)
    {
        CurrentStationManager stationManager = carController.GetCurrentStationManager();
        bool isOnFloorOne = stationManager.GetCurrentElevatorFloor() == 1;
        bool isOnGroundFloor = stationManager.GetCurrentElevatorFloor() == 0;
        if (!taskComplete)
        {
            // Logica para dejar objetos
            if (!HasCarObject())
            {
                // There is no obj here and check if they are the same object
                if (isOnFloorOne && player.HasCarObject() && player.GetCarObject().GetObjectSO() == box && itHasDirtyOil)
                {
                    // El player tiene algo en la mano
                    player.GetCarObject().SetCarObjectParent(this);
                    StartCoroutine(TimeToRequest(timeRequest));
                    
                    

                }
                //Verifica si el player tiene un objeto, si es un aceite y el auto no tiene aceite sucio. Si se cumple completa la tarea
                else if (isOnGroundFloor && player.HasCarObject() && player.GetCarObject().GetObjectSO() == oil && !itHasDirtyOil)
                {
                    player.GetCarObject().SetCarObjectParent(this);

                    taskComplete = true;
                    Destroy(GetCarObject().gameObject);
                }
                else
                {
                    // Player no tiene nada en la mano

                }
            }
            else
            {
                if (!player.HasCarObject() && !itHasDirtyOil)
                {
                    // Player is not carrying anything. He only takes it when he finishes the task.

                    GetCarObject().SetCarObjectParent(player);

                }

            }
        }
    }

    IEnumerator TimeToRequest(float timeRequest)
    {
        yield return new WaitForSeconds(timeRequest);
        itHasDirtyOil = false;
        //Limpia el carObject de la mesa y lo destruye.
        Destroy(GetCarObject().gameObject);
        ClearCarObject();
        Transform boxFullPreFab = Instantiate(boxFull.prefab, GetCarObjectFollowTransform());
        boxFullPreFab.GetComponent<CarObject>().SetCarObjectParent(this);
    }
}
