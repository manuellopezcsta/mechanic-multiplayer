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
    private bool inTutorial = false;
    void Start()
    {
        inTutorial = TutorialManagerOilChange.Instance != null;
    }
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
                if (player.GetCarObject().GetObjectSO().name == "CajaFull" && inTutorial)
                {
                    TutorialManagerOilChange.Instance.StateChange(TutorialManagerOilChange.StateTutorial.Trash, TutorialManagerOilChange.StateTutorial.ElevatorBottom);
                }
                player.GetCarObject().SetCarObjectParent(this);
                //ejecutamos la funcion de destruir del objeto
                GetCarObject().DestroySelf();
                ClearCarObject();
                OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
