using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private GameObject posSalida;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Ingreso");
            Debug.Log(other.transform.position);
            other.transform.parent = null;
            other.GetComponent<CharacterController>().enabled = false; // Desactiva el character controller para mover la posicion del player
            other.transform.position = posSalida.transform.position; //Cambia la posicion del player a la del posSalida
            other.GetComponent<CharacterController>().enabled = true; //La vuelve a activar
            Debug.Log(other.transform.position);
        }
    }
}
