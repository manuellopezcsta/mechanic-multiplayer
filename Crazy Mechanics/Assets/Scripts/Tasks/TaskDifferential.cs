using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaskDifferential : BaseCounter, IHasProgress
{
    public CarController carController; // Se asigna cuando se crea mediante el StationManager.
    [SerializeField] private TaskIndicatorUI indicatorUI;
    [SerializeField] private int score;
    [SerializeField] private ObjectsSO fixingTool;
    private int fixingProgress;
    [SerializeField] private int fixingProgressMax = 10;
    private CurrentStationManager stationManager;

    public static event EventHandler OnFixingDiff;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public void Start()
    {
        stationManager = carController.GetCurrentStationManager();
        Debug.Log("Current StationManager: " + stationManager);
    }

    public override void Interact(Player player)
    {
        // Si el player tiene la herramienta para arreglarlo.
        if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool && fixingProgress < fixingProgressMax && stationManager.GetCurrentElevatorFloor() == 2)
        {
            fixingProgress++;
            OnFixingDiff?.Invoke(this, EventArgs.Empty);
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)fixingProgress / fixingProgressMax
            });
        }

        // Si completamos la tarea
        if (fixingProgress == fixingProgressMax)
        {
            transform.GetComponent<BoxCollider>().enabled = false;
            carController.AddScoreTask(score);
            indicatorUI.SetAsComplete();
            carController.CompleteTask();
        }
    }
}
