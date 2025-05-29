
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CurrentStationManager : MonoBehaviour
{
    public CarController currentCar; // Guarda el autoActual. LIMPIARLO AL TERMINAR para indicar que esta libre.
    [SerializeField] ElevatorController elevatorController;
    [SerializeField] MotorToolDocking docking;
    [SerializeField] private int elevatorNumber = 1;
    private bool isElevatorLocked = false; // Si el elevador esta lockeado porque esta en uso
    [SerializeField] ButtonStart buttonStart;

    public static event EventHandler OnCarDelivery;


    public bool IsElevatorLocked() {
        return isElevatorLocked;
    }

    public void LockAndUnlockElevator() {
        isElevatorLocked = !isElevatorLocked;
    }

    public int GetCurrentElevatorFloor()
    {
        return elevatorController.floorNumberElevator;
    }

    // Motor tool = pluma
    public bool IsMotorToolDocked()
    {
        if (docking == null)
        {
            return false;
        }
        else
        {
            return docking.isMotorToolDocked();
        }
    }

    public int getStationNumber()
    {
        return elevatorNumber;
    }

    public MotorTool GetCurrentMotorTool()
    {
        if (docking == null)
        {
            return null;
        }
        if (docking.GetCurrentMotorTool() != null)
        {
            return docking.GetCurrentMotorTool();
        }
        else
        {
            //Debug.LogError("MOTOR TOOL ES NULL!");
            return null;
        }
    }

    public bool isFree()
    {
        return currentCar == null  && elevatorController.floorNumberElevator == 0;
    }

    public void FreeStation()
    {
        currentCar = null;
        //Debug.Log("Se libera la estacion");
    }

    public void SetCarToStation(CarController car) {
        currentCar = car;
    }


    public void TryToDeliverCar()
    {
        if (currentCar != null && currentCar.carFixed && GetCurrentElevatorFloor() == 0 && !elevatorController.CheckIfElevatorIsMoving())
        {
           //Agregar score total al scoremanager
            ScoreManager.Instance.AddPoints(currentCar.carScoreValue);
            ScoreManager.Instance.CarsDelivered();
            currentCar.canMove = true;
            FreeStation();
            Debug.Log("Se entrego el auto");
            OnCarDelivery?.Invoke(this, EventArgs.Empty);
            // Prendemos las particulas de entrega
            DevileryCounter.Instance?.ShowMoneyParticles();
        }
    }
    // GAME MANAGER, CREA AUIO EN UNO DE LOS elevadores.. y le asigna un manager?.
}
