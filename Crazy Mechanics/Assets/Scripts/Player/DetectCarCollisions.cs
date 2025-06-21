using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCarCollisions : MonoBehaviour
{
    private Player player;
    private PlayerMovement playerMovement;
    const string CAR_TAG = "Car";
    const string BOTTOM_CAR = "BottomCar";

    private void Start()
    {
        player = GetComponentInParent<Player>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    // Si al trigger del player, entra el frente del auto, (el cual tiene un collider con tag), se trigerea el stun.
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        // Stun por auto chocando
        if (other.CompareTag(CAR_TAG) && other.GetComponentInParent<CarController>().canMove) {
            //Debug.Log("Colisiono con el auto");
            playerMovement.RespawnAtPos();
            StartCoroutine(playerMovement.GetStunned());
        }
        // Stun por objeto arrojado
        if(other.TryGetComponent<InvisibleHolder>(out InvisibleHolder invisibleHolder)){
            if(invisibleHolder.flying && invisibleHolder.thrownBy != player){
                StartCoroutine(playerMovement.GetStunned());
            }
        }
        // Stun x estar abajo del auto
        if(other.CompareTag(BOTTOM_CAR)){
            Debug.Log("Entro en trigger");
            playerMovement.RespawnAtPos();
            StartCoroutine(playerMovement.GetStunned());
        }
    }
}
