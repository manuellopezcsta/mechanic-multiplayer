using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SwapBatteryTask : BaseCounter
{
    public CarController carController;
    [SerializeField] private CarObject battery;

    [SerializeField] private ObjectsSO chagedBattery;
    [SerializeField] public bool taskComplete;

    [SerializeField] private TaskIndicatorUI indicatorUI;
    [SerializeField] private int score;


    private void Start() {
        SetCarObject(battery);
        GetCarObject().SetCarObjectParent(this);
    }
    public override void Interact(Player player)
    {
        CurrentStationManager stationManager = carController.GetCurrentStationManager();

        //Debug.Log("Interactua con bateria");
        // Logica para dejar objetos
        if (!HasCarObject()) {
            // There is no obj here and check if they are the same object
            if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == chagedBattery) {
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
                // Lo borramos.
                Destroy(GetCarObject().gameObject);
                // Hacemos sonido
                SoundManager.Instance.PlayObjectDroppedSound(transform);
                // Marcamos la task como completa.
                taskComplete = true;
                carController.AddScoreTask(score);
                indicatorUI.SetAsComplete();
                carController.CompleteTask();
            } 
        } else {
            // There is a car obj here already.
            if(!player.HasCarObject() && !taskComplete) {
                GetCarObject().SetCarObjectParent(player);
            } 
        }
    }
}
