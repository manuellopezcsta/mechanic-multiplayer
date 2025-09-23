using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private ObjectsSO objectsSO;
    private int itemSpawnLimit;
    private bool inTutorial=false;

    private void Start()
    {
        itemSpawnLimit = SpawnLimitManager.Instance.GetItemSpawnLimit(objectsSO.name);
        TutorialCheck();
    }

    public override void Interact(Player player)
    {
        //ademas de preguntar si el player no tiene un objeto verificamos que los objetos generados sean menores que el maximo permitido
        if (!player.HasCarObject() && itemSpawnLimit > SpawnLimitManager.Instance.GetSpawnedItemsCount(objectsSO.name))
        {
            // Si el player no tiene nada en la mano, spawneamos 1
            Transform carObjectTransform = Instantiate(objectsSO.prefab);
            carObjectTransform.GetComponent<CarObject>().SetCarObjectParent(player);
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
            SpawnLimitManager.Instance.ModifySpawnedCounter(objectsSO.name, 1);
            if (inTutorial) {
                TutorialStateChange();
            }
        }
    }
    //Chequea si se esta en esta involucrado en el tutorial del nivel
    public void TutorialCheck()
    {
        switch (objectsSO.name)
        {
            case "Wheel":
                inTutorial = TutorialManagerWheel.Instance != null;
                break;
            case "Caja":
                inTutorial = TutorialManagerOilChange.Instance != null;
                break;
            case "Aceite":
                inTutorial = TutorialManagerOilChange.Instance != null;
                break;
            //Completar los demas casos
            default:
                break;
        }
    }
    //realiza el cambio de estado correspondiente
    public void TutorialStateChange()
    {
        switch (objectsSO.name)
        {
            case "Wheel":
                TutorialManagerWheel.Instance.StateChange(TutorialManagerWheel.StateTutorial.WheelPileArrow, TutorialManagerWheel.StateTutorial.BalanceToolArrow);
                break;
            case "Caja":
                TutorialManagerOilChange.Instance.StateChange(TutorialManagerOilChange.StateTutorial.Boxes, TutorialManagerOilChange.StateTutorial.Task);
                break;
            case "Aceite":
                TutorialManagerOilChange.Instance.StateChange(TutorialManagerOilChange.StateTutorial.OilSpawner, TutorialManagerOilChange.StateTutorial.Task);
                break;
            //Completar demas casos
            default:
                break;
        }
    }
}
