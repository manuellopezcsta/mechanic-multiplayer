using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldThropies : MonoBehaviour
{
    [SerializeField] private Material colorThropies;
    [SerializeField] private Material colorLevelPlayed;
    [SerializeField] private GameObject levelIcon;
    [SerializeField] private GameObject[] thropies;
    [SerializeField] private string levelNumber;

    public void Awake()
    {
        int score = PlayerPrefs.GetInt(levelNumber,0); //variable puntaje es igual a la informacion guardada en levelNumber en caso contrario es 0
        if (score > 0) 
        {
            //Si el score es mayor que 0 entonces es nivel ah sido jugado y cambia el color del icono de nivel
            levelIcon.GetComponent<MeshRenderer>().material = colorLevelPlayed;
        }

        for (int i = 0; i < score; i++) //revisa cuantos trofeos hay, si hay 0 no hace nada, si hay 1 o mas cambia el material
        {
            thropies[i].GetComponent<MeshRenderer>().material = colorThropies;
        }
    }
}
    
