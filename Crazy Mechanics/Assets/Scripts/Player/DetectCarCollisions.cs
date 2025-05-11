using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCarCollisions : MonoBehaviour
{
    private Player player;
    const string CAR_TAG = "Car";
    const string BOTTOM_CAR = "BottomCar";

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    // Si al trigger del player, entra el frente del auto, (el cual tiene un collider con tag), se trigerea el stun.
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        // Stun por auto chocando
        if (other.CompareTag(CAR_TAG) && other.GetComponentInParent<CarController>().canMove) {
            //Debug.Log("Colisiono con el auto");
            player.RespawnAtRandomPos();
            StartCoroutine(player.GetStunned());
        }
        // Stun por objeto arrojado
        if(other.TryGetComponent<InvisibleHolder>(out InvisibleHolder invisibleHolder)){
            if(invisibleHolder.flying && invisibleHolder.thrownBy != player){
                StartCoroutine(player.GetStunned());
            }
        }
        // Stun x estar abajo del auto
        if(other.CompareTag(BOTTOM_CAR)){
            Debug.Log("Entro en trigger");
            player.RespawnAtRandomPos();
            StartCoroutine(player.GetStunned());
        }
    }

}
