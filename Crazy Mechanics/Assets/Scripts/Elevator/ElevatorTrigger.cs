using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ElevatorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject targetElevator;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            //Asegurar que el elevador este en 0 para que el auto pueda subir /no se empale.
            //other.GetComponent<BoxCollider>().enabled = false;
            other.GetComponent<CarController>().canMove = false;
            other.GetComponent<CarController>().taskComplete = false;
            Vector3 globalPosition = other.transform.position;
            other.transform.SetParent(targetElevator.transform);
            other.transform.position = globalPosition;
            Destroy(other.GetComponent<Rigidbody>());
            other.GetComponent<CarController>().TurnOnTasksColliders();
            //ChangeValueCollider();
            Debug.Log("Entro en el trigger");
        }
    }

    void OnTriggerExit(Collider other)
    {
       if(other.CompareTag("Car") && other.GetComponent<CarController>().taskComplete){
        other.transform.SetParent(null);
        Debug.Log("Salio del trigger");
        //ChangeValueCollider();
        other.AddComponent<Rigidbody>();
   
       } 
    }

    public void ChangeValueCollider(){
        if(gameObject.GetComponent<Collider>().enabled){
            gameObject.GetComponent<Collider>().enabled = false;
        }else{
            gameObject.GetComponent<Collider>().enabled = true;
        }
    }
}
