using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class ConveyorCounter : ClearCounter
{
    private float speed;
    private Transform startPoint;
    [SerializeField] private Transform[] destination;
    [SerializeField] private int currentDestination = 0;
    private bool destroyObject;
    const string TOOL_TAG = "Tool";
    const string CONVEGOR_END_NAME = "ConveyorEnd";
    void Update()
    {
         if (destination.Length == 0) return;

        Transform targetPoint = destination[currentDestination];
        transform.position = Vector3.MoveTowards(transform.position, destination[currentDestination].transform.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            currentDestination = (currentDestination + 1) % destination.Length; // Avanza al siguiente y vuelve al inicio si llega al final
        }

    }
    public void setUP(Transform newStartPoint, Transform[] newDestination, float newSpeed, bool canIDestroyObject)
    {
        startPoint = newStartPoint;
        destination = newDestination;
        speed = newSpeed;
        destroyObject = canIDestroyObject;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("entered somewhere");
        if (other.CompareTag(CONVEGOR_END_NAME))
        {
            if (HasCarObject() && !GetCarObject().CompareTag(TOOL_TAG) && destroyObject)
            {
                GetCarObject().DestroySelf();
            }
            currentDestination = 0;
            transform.position = startPoint.transform.position;
            gameObject.SetActive(false);
        }
    }

}
