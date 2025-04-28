using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private ObjectsSO objectsSO;
    private int itemSpawnLimit;

    private void Start()
    {
        itemSpawnLimit = SpawnLimitManager.Instance.GetItemSpawnLimit(objectsSO.name);
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
        }
    }
}
