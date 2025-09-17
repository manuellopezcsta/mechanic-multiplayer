using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TaskWheel : BaseCounter
{
    public CarController carController;
    [SerializeField] private ObjectsSO balancedWheel;
    [SerializeField] private CarObject wheel;
    [SerializeField] public bool taskComplete;
    [SerializeField] TaskIndicatorUI indicatorUI;
    [SerializeField] private int score;
    private bool inTutorial;

    CurrentStationManager stationManager;



    private void Start() {
        // Seteamos la rueda dentro del auto
        SetCarObject(wheel);
        GetCarObject().SetCarObjectParent(this);
        inTutorial = TutorialManagerWheel.Instance.tutorialEnabled;
        if (inTutorial)
        {
         TutorialManagerWheel.Instance.FindWheelTaskArrow();
        }

        // Decidimos con un random si va a tener rueda o si necesita una nueva.
        int random = Random.Range(0,2);
        if (random == 1)
        {
            SetCarObject(wheel);
            wheel.transform.position = transform.parent.gameObject.transform.position;
            if (inTutorial)
            {
                TutorialManagerWheel.Instance.StateChange(TutorialManagerWheel.StateTutorial.Idle, TutorialManagerWheel.StateTutorial.FlechaAuto);
            }
            // Si le falta la rueda.
        }
        else
        {
            ClearCarObject();
            wheel.gameObject.SetActive(false);
            Destroy(wheel.gameObject);
            indicatorUI.SwapToMissingWheelIcon();
            if (inTutorial)
            {
                TutorialManagerWheel.Instance.StateChange(TutorialManagerWheel.StateTutorial.Idle, TutorialManagerWheel.StateTutorial.FlechaPilaRueda);
            }
        }

        // Apagamos el mesh de las ruedas del auto del prefab para que solo se vean las ruedas instanciadas.
        transform.parent.gameObject.GetComponent<MeshRenderer>().enabled = false;

        stationManager = carController.GetCurrentStationManager();
    }

    public override void Interact(Player player)
    {   
        //Debug.Log("EntroAlInteract");

        // Logica para dejar objetos
        if (!HasCarObject()) {
            // There is no obj here and check if they are the same object
            if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == balancedWheel && stationManager.GetCurrentElevatorFloor() == 0)
            {
                ComboManager.Instance.UpdateCombo();
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
                // Prendemos la visual de la rueda arreglada y destruimos el gameObject.
                transform.parent.gameObject.GetComponent<MeshRenderer>().enabled = true;
                GetCarObject().DestroySelf();
                // Hacemos sonido
                SoundManager.Instance.PlayObjectDroppedSound(transform);
                taskComplete = true;
                carController.AddScoreTask(score);
                indicatorUI.SetAsComplete();
                carController.CompleteTask();
                //Apagamos el collider para que no moleste para otras tasks
                transform.GetComponent<BoxCollider>().enabled = false;
                TutorialManagerWheel.Instance.StateChange(TutorialManagerWheel.StateTutorial.FlechaAuto, TutorialManagerWheel.StateTutorial.Idle);
            }
        } else {
            // Logica para sacar la rueda del auto.
            if (!player.HasCarObject() && !taskComplete && stationManager.GetCurrentElevatorFloor() == 0)
            {
                ComboManager.Instance.UpdateCombo();
                indicatorUI.SwapToMissingWheelIcon();
                GetCarObject().SetCarObjectParent(player);
                SpawnLimitManager.Instance.ModifySpawnedCounter(wheel.GetObjectSO().name, 1);
                TutorialManagerWheel.Instance.StateChange(TutorialManagerWheel.StateTutorial.FlechaAuto, TutorialManagerWheel.StateTutorial.FlechaBalanceo);
            } 
        }
    }
}
