using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ConveyorCounter : ClearCounter
{
    private Transform startPoint;
    private Transform destination;
    private float speed;
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination.transform.position, speed * Time.deltaTime);
    }
    public void setUP(Transform newStartPoint, Transform newDestination, float newSpeed)
    {
        startPoint = newStartPoint;
        destination = newDestination;
        speed = newSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered somewhere");
        if (other.CompareTag("ConveyorEnd"))
        {
            transform.position = startPoint.transform.position;
            gameObject.SetActive(false);
        }
    }

}
