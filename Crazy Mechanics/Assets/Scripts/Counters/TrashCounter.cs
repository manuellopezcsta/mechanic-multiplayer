using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    [SerializeField] private List<ObjectsSO> tools;

    public static event EventHandler OnAnyObjectTrashed;
    [SerializeField] private int scoreTrash;
    public override void Interact(Player player)
    {

        // Logica para dejar objetos
        if (!HasCarObject())
        {
            // There is no obj here and check if they are the same object
            if (player.HasCarObject() && !tools.Contains(player.GetCarObject().GetObjectSO()))
            {
                ScoreManager.Instance.AddPoints(scoreTrash);
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
                //ejecutamos la funcion de destruir del objeto
                GetCarObject().DestroySelf();
                ClearCarObject();
                OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
