using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskMotor : BaseCounter
{
    public CarController carController;
    [SerializeField] public bool taskComplete = false;
    [SerializeField] ObjectsSO fixedMotor;
    [SerializeField] private CarObject motor;
    [SerializeField] private GameObject motorVisual;
    [SerializeField] private TaskIndicatorUI indicatorUI;
    [SerializeField] private int score;

    void Start()
    {
        SetCarObject(motor);
        GetCarObject().SetCarObjectParent(this);
    }
    public override void Interact(Player player)
    {
        CurrentStationManager stationManager = carController.GetCurrentStationManager();
        //Debug.Log("Se interacciono con el taskMotor. " + (stationManager!= null).ToString());
        bool conditionsMet = stationManager.GetCurrentElevatorFloor() == 0 && stationManager.IsMotorToolDocked();
        MotorTool motorTool = stationManager.GetCurrentMotorTool();

        if (!taskComplete)
        {
            // Logica para dejar objetos
            if (!HasCarObject() && motorTool != null)
            {
                // There is no obj here and the pluma has a fixedMotor.
                if (motorTool.HasCarObject() && motorTool.GetCarObject().GetObjectSO() == fixedMotor && !player.HasCarObject())
                {
                    //Ponemos el motor arreglado y marcamos la tarea como completa
                    motorTool.GetCarObject().SetCarObjectParent(this);
                    // Hacemos sonido
                    SoundManager.Instance.PlayObjectDroppedSound(transform);
                    taskComplete = true;
                    carController.AddScoreTask(score);
                    indicatorUI.SetAsComplete();
                    carController.CompleteTask();
                    // Escondemos el motor en la pluma y la reseteamos
                    motorTool.FinishFixing();
                    // Lo borramos x las dudas 
                    Destroy(GetCarObject().gameObject);
                }
            }
            else // Logica para sacar algo
            {
                //Debug.Log("conditionsMet" + conditionsMet.ToString() + " " + (stationManager.GetCurrentElevatorFloor() == 0).ToString() + " " +
                //(stationManager.IsMotorToolDocked()).ToString());
                if (conditionsMet && !player.HasCarObject())
                {
                    motorVisual.SetActive(true);
                    // El piso es el correcto y esta la pluma dockeada.
                    // Seteo el objeto al motor tool.
                    GetCarObject().SetCarObjectParent(motorTool);
                    motorTool.ShowMotor();
                    // Hacemos sonido
                    SoundManager.Instance.PlayObjectDroppedSound(transform);
                }
            }
        }
    }

    // REVISAR ESTO y PONER EN LA RUTINA PARA QUE TARDE EN PONER Y SACAR MOTOR.
    IEnumerator TimeToRequest(float timeRequest, CurrentStationManager csm)
    {
        // Lockeamos el elevador
        csm.LockAndUnlockElevator();
        yield return new WaitForSeconds(timeRequest);
        //Limpia el carObject de la mesa y lo destruye.
        Destroy(GetCarObject().gameObject);
        ClearCarObject();
        Transform boxFullPreFab = Instantiate(fixedMotor.prefab, GetCarObjectFollowTransform());
        boxFullPreFab.GetComponent<CarObject>().SetCarObjectParent(this);
        // Liberamos el elevador
        csm.LockAndUnlockElevator();
    }
}
