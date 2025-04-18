using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class CarController : MonoBehaviour
{
    const string STOP_TAG = "Stop";
    private CurrentStationManager currentStationManager;

    public bool carFixed = false;
    public bool canMove = true;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float targetZ = 10f;
    [SerializeField] private List<GameManager.CarTasks> carTasks;


    // Posiciones para spawnear las tasks
    [SerializeField] private Transform oilTaskPosition;
    [SerializeField] private Transform motorTaskPosition;
    [SerializeField] private Transform batteryTaskPosition;

    [SerializeField] private Transform wheelFRPosition;
    [SerializeField] private Transform wheelFLPosition;
    [SerializeField] private Transform wheelBRPosition;
    [SerializeField] private Transform wheelBLPosition;
    
    // Puntaje por auto
    [SerializeField] public int carScoreValue = 0;

    // Las tasks que se crearon
    private List<GameObject> createdTasksContainers = new List<GameObject>();

    private int completedTasksCounter = 0;
    void Update()
    {
        if (canMove)
        {
            // El auto va hacia adelante.
            Vector3 currentPosition = transform.position;
            float newZ = Mathf.MoveTowards(currentPosition.z, targetZ, speed * Time.deltaTime);
            transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);
        }
        if (transform.position.z <= targetZ)
        {
            Destroy(gameObject);
        }
    }

    public void TurnOnTasksColliders()
    {
        foreach (GameObject task in createdTasksContainers)
        {
            task.GetComponent<BoxCollider>().enabled = true;
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        // Esto esta para que no choque con la pared cuando se instancia el auto, se prenden los collider de las tasks, despues.
        if(other.CompareTag(STOP_TAG)) {   
            //Debug.Log("Prendiendo colliders");
            TurnOnTasksColliders();
        }

    }*/

    public void SetCurrentStationManager(CurrentStationManager target)
    {
        currentStationManager = target;
    }

    public CurrentStationManager GetCurrentStationManager()
    {
        return currentStationManager;
    }

    public void GenerateTask(GameManager.CarTasks task)
    {
        // Spawneamos las tasks y les cambiamos la posicion a donde corresponde.
        GameObject generatedTask = null;

        switch (task)
        {
            default:
                Debug.LogError("NO EXISTE ESTA TASK!");
                return;
            case GameManager.CarTasks.OIL_CHANGE:
                generatedTask = GameManager.Instance.GetOilTaskPrefab();
                generatedTask.transform.position = oilTaskPosition.position;
                generatedTask.transform.SetParent(oilTaskPosition);
                generatedTask.GetComponent<TaskOil>().carController = this;
                carTasks.Add(GameManager.CarTasks.OIL_CHANGE);
                createdTasksContainers.Add(generatedTask);
                return;
            case GameManager.CarTasks.MOTOR_FIX:
                generatedTask = GameManager.Instance.GetMotorTaskPrefab();
                generatedTask.transform.position = motorTaskPosition.position;
                generatedTask.transform.SetParent(motorTaskPosition);
                generatedTask.GetComponent<TaskMotor>().carController = this;
                carTasks.Add(GameManager.CarTasks.MOTOR_FIX);
                createdTasksContainers.Add(generatedTask);
                return;
            case GameManager.CarTasks.WHEEL_FIX_FR:
                generatedTask = GameManager.Instance.GetWheelTaskPrefab();
                generatedTask.transform.position = wheelFRPosition.position;
                generatedTask.transform.SetParent(wheelFRPosition);
                generatedTask.GetComponent<TaskWheel>().carController = this;
                carTasks.Add(GameManager.CarTasks.WHEEL_FIX_FR);
                createdTasksContainers.Add(generatedTask);
                return;
            case GameManager.CarTasks.WHEEL_FIX_FL:
                generatedTask = GameManager.Instance.GetWheelTaskPrefab();
                generatedTask.transform.position = wheelFLPosition.position;
                generatedTask.transform.SetParent(wheelFLPosition);
                generatedTask.GetComponent<TaskWheel>().carController = this;
                carTasks.Add(GameManager.CarTasks.WHEEL_FIX_FL);
                createdTasksContainers.Add(generatedTask);
                return;
            case GameManager.CarTasks.WHEEL_FIX_BR:
                generatedTask = GameManager.Instance.GetWheelTaskPrefab();
                generatedTask.transform.position = wheelBRPosition.position;
                generatedTask.transform.SetParent(wheelBRPosition);
                generatedTask.GetComponent<TaskWheel>().carController = this;
                carTasks.Add(GameManager.CarTasks.WHEEL_FIX_BR);
                createdTasksContainers.Add(generatedTask);
                return;
            case GameManager.CarTasks.WHEEL_FIX_BL:
                generatedTask = GameManager.Instance.GetWheelTaskPrefab();
                generatedTask.transform.position = wheelBLPosition.position;
                generatedTask.transform.SetParent(wheelBLPosition);
                generatedTask.GetComponent<TaskWheel>().carController = this;
                carTasks.Add(GameManager.CarTasks.WHEEL_FIX_BL);
                createdTasksContainers.Add(generatedTask);
                return;
            case GameManager.CarTasks.BATTERY_CHARGE:
                generatedTask = GameManager.Instance.GetBatteryTaskPrefab();
                generatedTask.transform.position = batteryTaskPosition.position;
                generatedTask.transform.SetParent(batteryTaskPosition);
                generatedTask.GetComponent<SwapBatteryTask>().carController = this;
                carTasks.Add(GameManager.CarTasks.BATTERY_CHARGE);
                createdTasksContainers.Add(generatedTask);
                return;
        }
       
    }
    private void CheckIfFinishedFixing(){
        if(createdTasksContainers.Count == completedTasksCounter){
            carFixed = true;
            Debug.Log("Auto esta listo para entregar");
        }
    }
    public void CompleteTask(){
    //aumenta las tareas completadas en 1, se llama cada vez que una tarea se completa, y esta vinculada al auto. 
        completedTasksCounter ++;
        Debug.Log("Se aumento las tareas completadas " + completedTasksCounter  );
        // Nos fijamos si el auto esta listo.
        CheckIfFinishedFixing();
    }

    public void AddScoreTask(int score){
        carScoreValue += score;
    }

}