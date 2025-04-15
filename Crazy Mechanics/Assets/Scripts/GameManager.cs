using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        LevelTimer levelTimer = GetComponent<LevelTimer>();
        levelTimer.StartTimer();
    }

    // Tasks para los autos.
    [SerializeField] GameObject oilTaskPrefab;
    [SerializeField] GameObject motorTaskPrefab;
    [SerializeField] GameObject wheelTaskPrefab;
    [SerializeField] GameObject batteryTaskPrefab;


    // Lista de autos que puede spawnear
    [SerializeField] private GameObject[] carPrefabs;

    // Posiciones donde se instancian los autos
    [SerializeField] GameObject[] positionInstanciateCars;
    [SerializeField] CurrentStationManager[] stations;

    // Para cortar la luz
    [SerializeField] LightBoxController lightBoxController;



    public enum CarTasks
    {
        OIL_CHANGE,
        MOTOR_FIX,
        WHEEL_FIX_FR,
        WHEEL_FIX_FL,
        WHEEL_FIX_BR,
        WHEEL_FIX_BL,
        BATTERY_CHARGE,
    }

    public void GenerateCar(int numberOfTaks)
    {
        // Elijo un auto random de los que tengo
        int index = UnityEngine.Random.Range(0, carPrefabs.Length);
        GameObject car = Instantiate(carPrefabs[index]);
        int elevatorNumber = UnityEngine.Random.Range(0, positionInstanciateCars.Length);
        // Le asigno la posicion de spawneo y su stationManager.
        car.transform.position = positionInstanciateCars[elevatorNumber].transform.position;
        CarController controller = car.GetComponent<CarController>();
        controller.SetCurrentStationManager(stations[elevatorNumber]);
        //Generamos las tasks que queremos.
        List<CarTasks> tasksToDo = ChooseRandomTasks(numberOfTaks);
        foreach (CarTasks task in tasksToDo)
        {
            controller.GenerateTask(task);
        }
        // Dejamos que se mueva
        controller.canMove = true;
    }


    // Retornamos los prefabs para que los tengan los car controller
    public GameObject GetOilTaskPrefab()
    {
        return Instantiate(oilTaskPrefab);
    }

    public GameObject GetMotorTaskPrefab()
    {
        return Instantiate(motorTaskPrefab);
    }
    public GameObject GetBatteryTaskPrefab()
    {
        return Instantiate(batteryTaskPrefab);
    }

    public GameObject GetWheelTaskPrefab()
    {
        // FIX TEMPORAL
        //return Instantiate(wheelTaskPrefab);
        return Instantiate(wheelTaskPrefab);
    }

    public int GetTaskCount()
    {
        return Enum.GetValues(typeof(CarTasks)).Length;
    }

    public List<CarTasks> ChooseRandomTasks(int ammount)
    {
        if (ammount < 1 || ammount > Enum.GetValues(typeof(CarTasks)).Length)
        {
            throw new ArgumentException("La cantidad debe estar entre 1 y 6.");
        }

        // Obtener todos los elementos del enum y convertirlos a una lista.
        List<CarTasks> output = new List<CarTasks>((CarTasks[])Enum.GetValues(typeof(CarTasks)));

        // Mezclar la lista.
        System.Random random = new System.Random();
        for (int i = output.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            CarTasks temp = output[i];
            output[i] = output[j];
            output[j] = temp;
        }

        // Retornar la cantidad deseada de elementos.
        return output.GetRange(0, ammount);
    }

    public bool IsPowerEnabled()
    {
        return !lightBoxController.IsPowerDown();
    }

    public bool CanSpawnCar() {
        // Checkeamos todas las stations a ver si hay una libre para spawnear.
        foreach(CurrentStationManager station in stations) {
            if(station.isFree()) {
                return true;
            }
        }
        return false;
    }


}
