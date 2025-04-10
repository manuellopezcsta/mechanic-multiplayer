using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentStationManager : MonoBehaviour
{
    public CarController currentCar; // Guarda el autoActual. LIMPIARLO AL TERMINAR para indicar que esta libre.
    [SerializeField] ElevatorController elevatorController;
    [SerializeField] MotorToolDocking docking;
    private int elevatorNumber = 1;
    private bool isElevatorLocked = false; // Si el elevador esta lockeado porque esta en uso

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
        return currentCar == null;
    }

    public void FreeStation()
    {
        currentCar = null;
    }


    public void TryToDeliverCar()
    {
        if (currentCar != null && currentCar.carFixed)
        {
            currentCar.canMove = true;
            Debug.Log("Se entrego el auto");
        }
    }
    // GAME MANAGER, CREA AUIO EN UNO DE LOS elevadores.. y le asigna un manager?.
}
