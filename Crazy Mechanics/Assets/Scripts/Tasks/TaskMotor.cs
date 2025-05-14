using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskMotor : BaseCounter
{
    public CarController carController;
    CurrentStationManager stationManager;
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
        stationManager = carController.GetCurrentStationManager();
    }
    public override void Interact(Player player)
    {
        //Debug.Log("Se interacciono con el taskMotor. " + (stationManager!= null).ToString());
        bool conditionsMet = stationManager.GetCurrentElevatorFloor() == 0 && stationManager.IsMotorToolDocked();
        MotorTool motorTool = stationManager.GetCurrentMotorTool();

        if (!taskComplete)
        {
            // Logica para dejar objetos
            if (!HasCarObject() && motorTool != null)
            {
                // There is no obj here and the pluma has a fixedMotor.
                if (motorTool.HasCarObject() && motorTool.GetCarObject().GetObjectSO() == fixedMotor && !player.HasCarObject() && conditionsMet)
                {
                    ComboManager.Instance.UpdateCombo();
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
                    //Apagamos el collider para que no moleste para otras tasks
                    transform.GetComponent<BoxCollider>().enabled = false;
                }
            }
            else // Logica para sacar algo
            {
                //Debug.Log("conditionsMet" + conditionsMet.ToString() + " " + (stationManager.GetCurrentElevatorFloor() == 0).ToString() + " " +
                //(stationManager.IsMotorToolDocked()).ToString());
                if (conditionsMet && !player.HasCarObject() && !motorTool.HasCarObject())
                {
                    ComboManager.Instance.UpdateCombo();
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
}
