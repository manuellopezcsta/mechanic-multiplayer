using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevileryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        Debug.Log("Se entrego algo");
    }

    // Por ahi con el alternateInteract, podemos elegir cual queremos entregar ?
}
