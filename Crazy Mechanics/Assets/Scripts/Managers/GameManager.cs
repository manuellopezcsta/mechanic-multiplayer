using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        LevelTimer levelTimer = GetComponent<LevelTimer>();
        levelTimer.StartTimer(levelProperties.levelTime);
    }

    // Tasks para los autos.
    [SerializeField] TaskPrefabContainerSO tasksData;
    GameObject oilTaskPrefab;
    GameObject motorTaskPrefab;
    GameObject wheelTaskPrefab;
    GameObject batteryTaskPrefab;
    GameObject fixDiffTaskPrefab;
    GameObject unbendTaskPrefab;
    [SerializeField] LevelProperties levelProperties;
    [SerializeField] CarListSO carListSO;


    // Lista de autos que puede spawnear
    [SerializeField] private GameObject[] carPrefabs;

    // Posiciones donde se instancian los autos
    [SerializeField] GameObject[] positionInstanciateCars;
    [SerializeField] CurrentStationManager[] stations;

    // Para cortar la luz
    [SerializeField] LightBoxController lightBoxController;

    // Para el evento de select de los counter
    public static List<Player> playerList = new List<Player>();
    public static List<PlayerInputHandler> inputHandlersList = new List<PlayerInputHandler>();

    public Transform[] playerSpawns;

    public static event EventHandler OnCarSpawned;

    public enum CarTasks
    {
        OIL_CHANGE,
        MOTOR_FIX,
        WHEEL_FIX_FR,
        WHEEL_FIX_FL,
        WHEEL_FIX_BR,
        WHEEL_FIX_BL,
        BATTERY_CHARGE,
        DIFF_FIX,
        UNBEND,
    }

    void Start()
    {
        // NO TOCAR!
        PlayerConfigurationManager.Instance.SwitchInputMethod(true);
        // Cargamos los autos para este nivel usando el la lista correspondiente y la data del nivel
        carPrefabs = GetCarsForThisLevel();
        // Cargamos los prefabs de las tasks
        LoadTasksData();
    }

    private void LoadTasksData() {
        oilTaskPrefab = tasksData.oilTaskPrefab;
        motorTaskPrefab= tasksData.motorTaskPrefab;
        wheelTaskPrefab = tasksData.wheelTaskPrefab;
        batteryTaskPrefab = tasksData.batteryTaskPrefab;
        fixDiffTaskPrefab = tasksData.fixDiffTaskPrefab;
        unbendTaskPrefab = tasksData.unbendTaskPrefab;
    }

    public void GenerateCar()
    {
        // Elijo un auto random de los que tengo
        int index = UnityEngine.Random.Range(0, carPrefabs.Length);
        GameObject car = Instantiate(carPrefabs[index]);
        int elevatorNumber = UnityEngine.Random.Range(0, positionInstanciateCars.Length);
        // Le asigno la posicion de spawneo y su stationManager.
        car.transform.position = positionInstanciateCars[elevatorNumber].transform.position;
        CarController controller = car.GetComponent<CarController>();
        controller.SetCurrentStationManager(stations[elevatorNumber]);
        // Le asignamos el station para bloquear spawns.
        stations[elevatorNumber].SetCarToStation(controller);
        //Generamos las tasks que queremos.
        int randomTask = UnityEngine.Random.Range(levelProperties.minTaskNumber, levelProperties.maxTaskNumber + 1);
        List<CarTasks> tasksToDo = ChooseRandomTasks(randomTask);
        foreach (CarTasks task in tasksToDo)
        {
            controller.GenerateTask(task);
        }
        // Dejamos que se mueva
        controller.canMove = true;
        // Ejecutamos el evento para el sonido
        OnCarSpawned?.Invoke(this, EventArgs.Empty);
    }

    GameObject[] GetCarsForThisLevel() {
        GameObject[] output;
        var correctList = carListSO.levelList.FirstOrDefault(a => a.level == Convert.ToInt32(levelProperties.levelNumber));
        if (correctList != null) {
            output = correctList.cars.ToArray();
        } else {
            output = carListSO.allTheCars.ToArray();
        }
        return output;
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
        return Instantiate(wheelTaskPrefab);
    }

    public GameObject GetDiffTaskPrefab()
    {
        return Instantiate(fixDiffTaskPrefab);
    }

    public GameObject GetUnbendTaskPrefab(){
        return Instantiate(unbendTaskPrefab);
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
        List<CarTasks> output = levelProperties.listTasks;
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

    public LevelProperties GetLevelProperties() {
        return levelProperties;
    }

    public static void RegisterPlayer(Player player)
    {
        Debug.Log("Registrando Player.");
        playerList.Add(player);
        inputHandlersList.Add(player.gameObject.GetComponent<PlayerInputHandler>());
        //Debug.Log(inputHandlersList[0]);
    }

    private void OnDestroy() {
        playerList.Clear();
        inputHandlersList.Clear();
    }

    public static void NukePlayerControllers(){
        //Debug.Log(playerList.Count + " player list count");
        //Debug.Log(inputHandlersList.Count + " input handlers");
        //Debug.Log(playerList[0]);
        /*foreach(Player player in playerList) {
            player.gameObject.GetComponent<PlayerInputHandler>().UnsuscribeController();
        }*/
        foreach(PlayerInputHandler inputHandler in inputHandlersList) {
            inputHandler.UnsuscribeController();
        }
    }

}
