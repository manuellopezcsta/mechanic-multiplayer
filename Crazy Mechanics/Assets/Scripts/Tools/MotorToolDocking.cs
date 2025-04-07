using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorToolDocking : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float setSpeed;

    [SerializeField] private MotorTool currentMotorTool;

    
    public bool motorTooldocked = false;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Pluma")
        {
            motorTooldocked = true;
            Vector3 targetPos = transform.position + offset;
            other.transform.position = Vector3.Lerp(other.transform.position, targetPos, Time.deltaTime * setSpeed);
            if(currentMotorTool == null) {
                currentMotorTool = other.gameObject.GetComponent<MotorTool>();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Pluma")
        {
            motorTooldocked = false;
            currentMotorTool = null;
        }
    }

    public bool isMotorToolDocked() {
        return motorTooldocked;
    }

    public MotorTool GetCurrentMotorTool() {{
        return currentMotorTool;
    }}
}
