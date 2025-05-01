using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldThropies : MonoBehaviour
{
    [SerializeField] private Material color;
    [SerializeField] private GameObject[] thropies;
    [SerializeField] private string levelNumber;

    public void Awake()
    {
        int score = PlayerPrefs.GetInt(levelNumber,0); //variable puntaje es igual a la informacion guardada en levelNumber en caso contrario es 0

        for (int i = 0; i < score; i++) //revisa cuantos trofeos hay, si hay 0 no hace nada, si hay 1 o mas cambia el material
        {
            thropies[i].GetComponent<MeshRenderer>().material = color;
        }
    }
}
    
