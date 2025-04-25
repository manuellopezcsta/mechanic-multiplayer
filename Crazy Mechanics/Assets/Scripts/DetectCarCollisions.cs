using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCarCollisions : MonoBehaviour
{
    private Player playerParent;

    private void Start()
    {
        playerParent = GetComponentInParent<Player>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car") && other.GetComponentInParent<CarController>().canMove) {
            Debug.Log("Colisiono con el auto");
            playerParent.RespawnAtRandomPos();
            StartCoroutine(playerParent.GetStunned());
        }
    }

}
