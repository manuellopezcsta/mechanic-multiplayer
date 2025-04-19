using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ElevatorTrigger : MonoBehaviour
{
    const string CAR_TAG = "Car";
    [SerializeField] private GameObject targetElevator;
    [SerializeField] private CurrentStationManager currentStationManager;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CAR_TAG))
        {
            //Asegurar que el elevador este en 0 para que el auto pueda subir /no se empale.
            other.GetComponent<CarController>().canMove = false;
            Vector3 globalPosition = other.transform.position;
            other.transform.SetParent(targetElevator.transform);
            other.transform.position = globalPosition;
            Destroy(other.GetComponent<Rigidbody>());
            // Prendemos los colliders de las tasks
            other.GetComponent<CarController>().TurnOnTasksColliders();
            // Guardamos el auto en el Station Controller
            currentStationManager.currentCar = other.GetComponent<CarController>();
            //ChangeValueCollider();
            //Debug.Log("Entro en el trigger");
        }
    }

    public void ChangeValueCollider()
    {
        if (gameObject.GetComponent<Collider>().enabled)
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<Collider>().enabled = true;
        }
    }
}
