using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private ObjectsSO objectsSO;
    public override void Interact(Player player)
    {
        if (!player.HasCarObject())
        {
            // Si el player no tiene nada en la mano, spawneamos 1
            Transform carObjectTransform = Instantiate(objectsSO.prefab);
            carObjectTransform.GetComponent<CarObject>().SetCarObjectParent(player);

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}
