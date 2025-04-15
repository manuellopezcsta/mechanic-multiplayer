using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonStart : BaseCounter
{
    public override void Interact(Player player)
    {
        if (GameManager.Instance.CanSpawnCar())
        {
            //Random de task que tendra el auto
            int randomTasks = Random.Range(1, GameManager.Instance.GetTaskCount());
            Debug.Log("Cantidad de task en auto " + randomTasks);
            //GameManager.Instance.GenerateCar(randomTasks);
            GameManager.Instance.GenerateCar(1); // Para debugear mas facil.
        }
    }
}
