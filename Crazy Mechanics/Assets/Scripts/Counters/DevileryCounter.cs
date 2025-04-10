using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevileryCounter : BaseCounter
{
    [SerializeField] private List<CurrentStationManager> listElevators = new List<CurrentStationManager>();
    public override void Interact(Player player)
    {
        foreach (var currentStation in listElevators){
            currentStation.TryToDeliverCar();
            //Debug.Log("Se entrego algo");
        }
    }

    // Por ahi con el alternateInteract, podemos elegir cual queremos entregar ?
}
