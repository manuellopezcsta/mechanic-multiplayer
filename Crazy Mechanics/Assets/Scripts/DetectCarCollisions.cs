using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCarCollisions : MonoBehaviour
{
    private Player player;
    const string CAR_TAG = "Car";

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    // Si al trigger del player, entra el frente del auto, (el cual tiene un collider con tag), se trigerea el stun.
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if (other.CompareTag(CAR_TAG) && other.GetComponentInParent<CarController>().canMove) {
            //Debug.Log("Colisiono con el auto");
            player.RespawnAtRandomPos();
            StartCoroutine(player.GetStunned());
        }
    }
}
