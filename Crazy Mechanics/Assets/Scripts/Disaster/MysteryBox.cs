using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : BaseCounter
{
    public static event EventHandler OnOpenedMysteryBox;

    public override void Interact(Player player)
    {
        // Elije un random de 0 a 3 y dispara un evento.
        int evento = UnityEngine.Random.Range(0, 3);

        switch (evento) {
            // Disaster Velocidad
            case 0:
                DisasterManager.Instance.TriggerPlayerSpeedDisaster();
                break;
            // Disaster Cash
            case 1:
                DisasterManager.Instance.TriggerAddMoneyDisaster();
                break;
            // Disaster Controles
            case 2:
                DisasterManager.Instance.TriggerInvertControlsDisaster();
                break;
        }
        OnOpenedMysteryBox?.Invoke(this, EventArgs.Empty);

        Destroy(gameObject);
    }
}
