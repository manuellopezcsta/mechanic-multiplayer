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
    bool taskComplete;

    //public static event EventHandler OnFixingDiff;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    void Start()
    {
        stationManager = carController.GetCurrentStationManager();
        carBender = carController.GetComponent<CarBender>();
        // Abollamos el auto
        carBender.Bend();
    }


    public override void Interact(Player player)
    {
        // Si el player tiene la fixing tool, el elevador esta en 0 y no se termino de arreglar..
        if(player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool && stationManager.GetCurrentElevatorFloor() == 0 && fixingProgress < fixingProgressMax){
            fixingProgress ++;
            // Desdoblamos el auto un poquito. Agarramos el valor maximo que tiene deformado, lo hacemos positivo para que le sume y lo dividimos x el total para que quede en 0 si se hace esa cantidad de veces.
            carBender.Unbend((carBender.deformedNoiseValue *-1)/fixingProgressMax);
            // Disparamos el evento para la UI de progreso.
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)fixingProgress / fixingProgressMax
            });
        }
        // Si se termino de arreglar.
        if(fixingProgress == fixingProgressMax && !taskComplete){
            taskComplete = true;
            //Agregamos el puntaje 
            carController.AddScoreTask(score); 
            //Se setea la tarea como completada
            indicatorUI.SetAsComplete(); 
            carController.CompleteTask(); 
            //Apagamos el collider para que no moleste para otras tasks
            transform.GetComponent<BoxCollider>().enabled = false;
        }
    }

}
