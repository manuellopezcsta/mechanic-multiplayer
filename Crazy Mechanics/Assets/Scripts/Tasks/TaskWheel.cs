using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskWheel : BaseCounter
{
    public CarController carController;
    [SerializeField] private ObjectsSO balancedWheel;
    [SerializeField] private CarObject wheel;
    [SerializeField] public bool taskComplete;
    [SerializeField] TaskIndicatorUI indicatorUI;
    [SerializeField] private int score;



    private void Start() {
        // Seteamos la rueda dentro del auto
        SetCarObject(wheel);
        GetCarObject().SetCarObjectParent(this);

        // Decidimos con un random si va a tener rueda o si necesita una nueva.
        int random = Random.Range(0,2);
        if(random == 1){
           SetCarObject(wheel);
           wheel.transform.position = transform.parent.gameObject.transform.position;
           // Si le falta la rueda.
        }else{
            ClearCarObject();
            wheel.gameObject.SetActive(false);
            Destroy(wheel.gameObject);
            indicatorUI.SwapToMissingWheelIcon();
        }

        // Apagamos el mesh de las ruedas del auto del prefab para que solo se vean las ruedas instanciadas.
        transform.parent.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public override void Interact(Player player)
    {
        CurrentStationManager stationManager = carController.GetCurrentStationManager();
        
        //Debug.Log("EntroAlInteract");

        // Logica para dejar objetos
        if (!HasCarObject()) {
            // There is no obj here and check if they are the same object
            if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == balancedWheel) {
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
                taskComplete = true;
                carController.AddScoreTask(score);
                indicatorUI.SetAsComplete();
                carController.CompleteTask();
            }
        } else {
            // Logica para sacar la rueda del auto.
            if(!player.HasCarObject() && !taskComplete) {
                GetCarObject().SetCarObjectParent(player);
            } 
        }
    }
}
