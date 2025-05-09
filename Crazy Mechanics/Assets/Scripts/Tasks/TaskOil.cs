using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TaskOil : BaseCounter
{
    public CarController carController;
    [SerializeField] private ObjectsSO box;
    [SerializeField] private ObjectsSO oil;
    [SerializeField] private ObjectsSO boxFull;
    [SerializeField] public bool taskComplete;
    [SerializeField] private float timeRequest;
    [SerializeField] private bool itHasDirtyOil = true;
    [SerializeField] TaskIndicatorUI indicatorUI;
    [SerializeField] private int score;

    public static event EventHandler OnOilDraining;
    public static event EventHandler OnAddingOil;



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
                    ComboManager.Instance.UpdateCombo();
                    // El player tiene algo en la mano
                    player.GetCarObject().SetCarObjectParent(this);
                    // Empezamos a drenar el aceite sucio
                    // Hacemos sonido
                    SoundManager.Instance.PlayObjectDroppedSound(transform);
                    OnOilDraining?.Invoke(this, EventArgs.Empty);
                    StartCoroutine(TimeToRequest(timeRequest, stationManager));
                }
                //Verifica si el player tiene un objeto, si es un aceite y el auto no tiene aceite sucio. Si se cumple completa la tarea
                else if (isOnGroundFloor && player.HasCarObject() && player.GetCarObject().GetObjectSO() == oil && !itHasDirtyOil)
                {
                    ComboManager.Instance.UpdateCombo();
                    player.GetCarObject().SetCarObjectParent(this);
                    // Hacemos sonido
                    SoundManager.Instance.PlayObjectDroppedSound(transform);
                    OnAddingOil?.Invoke(this, EventArgs.Empty);
                    
                    taskComplete = true;
                    carController.AddScoreTask(score);
                    indicatorUI.SetAsComplete();
                    carController.CompleteTask();
                    // Destruimos el obj del aceite para que no se vea.
                    GetCarObject().DestroySelf();
                    //Apagamos el collider para que no moleste para otras tasks
                    transform.GetComponent<BoxCollider>().enabled = false;
                }
            }
            else // Logica para sacar el aceite Sucio del auto.
            {
                if (!player.HasCarObject() && !itHasDirtyOil)
                {
                    ComboManager.Instance.UpdateCombo();
                    // Player is not carrying anything. He only takes it when he finishes the task.
                    GetCarObject().SetCarObjectParent(player);
                    // Deslockeamos el elevador.
                    stationManager.LockAndUnlockElevator();

                }

            }
        }
    }

    IEnumerator TimeToRequest(float timeRequest, CurrentStationManager csm)
    {
        //Lockeamos el elevador por seguridad
        csm.LockAndUnlockElevator();
        yield return new WaitForSeconds(timeRequest);
        itHasDirtyOil = false;
        //Limpia el carObject de la mesa y lo destruye.
        GetCarObject().DestroySelf();
        Transform boxFullPreFab = Instantiate(boxFull.prefab, GetCarObjectFollowTransform());
        boxFullPreFab.GetComponent<CarObject>().SetCarObjectParent(this);
    }
}
