using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class CarController : MonoBehaviour
{
    public bool taskComplete = true;
    public bool canMove;
    [SerializeField] private float speed = 5f; 
    [SerializeField] private float targetZ = 10f; 

    void Update()
    {
        if (canMove && taskComplete){
        Vector3 currentPosition = transform.position;
        float newZ = Mathf.MoveTowards(currentPosition.z, targetZ, speed * Time.deltaTime);
        transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);
        }
        if(transform.position.z == targetZ){
            Destroy(gameObject);
        }
    }

}
