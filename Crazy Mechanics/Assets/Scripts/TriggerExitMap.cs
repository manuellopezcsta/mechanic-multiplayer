using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerExitMap : MonoBehaviour
{
    //Este script esta hecho para darle mejor experiencia de juego a los boludos que tiran las cosas fuera del nivel 
[SerializeField] private Transform LostAndFoundPosition;

    void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Car")){
            other.gameObject.transform.parent.transform.position = LostAndFoundPosition.position;
        }
    }
}
