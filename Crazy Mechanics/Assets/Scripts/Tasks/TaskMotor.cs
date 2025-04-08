using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMotor : BaseCounter
{
    public CarController carController;
    private bool taskComplete = false;

    [SerializeField] ObjectsSO fixedMotor;

    public override void Interact(Player player)
    {
        CurrentStationManager stationManager = carController.GetCurrentStationManager();
        Debug.Log("Se interacciono con el taskMotor. " + (stationManager!= null).ToString());
        bool conditionsMet = stationManager.GetCurrentElevatorFloor() == 0 && stationManager.IsMotorToolDocked();
        MotorTool motorTool = stationManager.GetCurrentMotorTool();


        if (!taskComplete)
        {
            // Logica para dejar objetos
            if (!HasCarObject() && motorTool != null)
            {
                Debug.Log("Entro hasta aca , armar objeto motorTool que contenga algo como motrorFixed");
                // There is no obj here and the pluma has a fixedMotor.
                if (motorTool.HasCarObject() && motorTool.GetCarObject().GetObjectSO() == fixedMotor)
                {
                    //Ponemos el motor arreglado y marcamos la tarea como completa
                    motorTool.GetCarObject().SetCarObjectParent(this);
                    taskComplete = true;
                    // Lo borramos x las dudas ??
                    Destroy(GetCarObject().gameObject);
                }
            }
            else // Logica para colocar algo adentro.
            {
                Debug.Log("conditionsMet" + conditionsMet.ToString() + " " + (stationManager.GetCurrentElevatorFloor() == 0).ToString() + " " +
                (stationManager.IsMotorToolDocked()).ToString());
                if (conditionsMet)
                {
                    Debug.Log("Entro hasta aca , fijarse que saque el objeto.. agregar car Object a este task cuando se inicia.");
                    // El piso es el correcto y esta la pluma dockeada.
                    
                    // Fijarse que el motor este seteado.

                    // Seteo el objeto al motor tool.
                    GetCarObject().SetCarObjectParent(motorTool);
                }
            }
        }
    }

    // REVISAR ESTO y PONER EN LA RUTINA PARA QUE TARDE EN PONER Y SACAR MOTOR.
    IEnumerator TimeToRequest(float timeRequest)
    {
        yield return new WaitForSeconds(timeRequest);
        //Limpia el carObject de la mesa y lo destruye.
        Destroy(GetCarObject().gameObject);
        ClearCarObject();
        Transform boxFullPreFab = Instantiate(fixedMotor.prefab, GetCarObjectFollowTransform());
        boxFullPreFab.GetComponent<CarObject>().SetCarObjectParent(this);
    }
}
