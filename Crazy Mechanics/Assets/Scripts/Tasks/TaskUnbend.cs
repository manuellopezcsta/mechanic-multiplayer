using System;
using System.Collections;
using System.Collections.Generic;
using Deform;
using Unity.VisualScripting;
using UnityEngine;

public class TaskUnbend : BaseCounter, IHasProgress
{
    public CarController carController; // Se asigna cuando se crea mediante el StationManager.
    [SerializeField] private TaskIndicatorUI indicatorUI;
    [SerializeField] private int score;
    [SerializeField] private ObjectsSO fixingTool;
    private int fixingProgress = 0;
    [SerializeField] private int fixingProgressMax = 10;
    private CurrentStationManager stationManager;
    private CarBender carBender;

    //public static event EventHandler OnFixingDiff;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    void Start()
    {
        stationManager = carController.GetCurrentStationManager();
        carBender = carController.GetComponent<CarBender>();
        carBender.Bend();
    }


    public override void Interact(Player player)
    {
        if(player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool && stationManager.GetCurrentElevatorFloor() == 0 && fixingProgress < fixingProgressMax){
            fixingProgress ++;
            carBender.UnBend(0.2f/fixingProgressMax);
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)fixingProgress / fixingProgressMax
            });
        }

        if(fixingProgress == fixingProgressMax){
            carController.AddScoreTask(score); //Agregamos el puntaje 
            indicatorUI.SetAsComplete(); //Se setea la tarea como completada
            carController.CompleteTask(); 
        }
    }

}
