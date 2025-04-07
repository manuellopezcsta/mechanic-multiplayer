using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] CurrentStationManager[] stations;

    public CurrentStationManager GetStationNumber(int stationNumber) {
        return stations[stationNumber];
    }

    // CREO AUTO CON PROBLEMAS

    // Lo asigno a una de las stations al car controller.

}
