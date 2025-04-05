using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private ObjectsSO objectsSO;

    public override void Interact(Player player)
    {
        // Logica para dejar objetos
        if (!HasCarObject()) {
            // There is no obj here
            if (player.HasCarObject()) {
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
            } else {
                // Player no tiene nada en la mano

            }
        } else {
            // There is a car obj here already.
            if(player.HasCarObject()) {
                // Player is carrying something
            } else {
                // Player is not carrying anything.
                GetCarObject().SetCarObjectParent(player);
            }
        }
    }
}
