using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWPlayerModel : MonoBehaviour
{
    private Vector3 direction;
    [SerializeField] private float speed;
    [SerializeField] private float smoothTurnigTime = 0.5f;
    private float currentVelocityTurn;

    public Vector3 CalculateMove(Vector2 inputVector)
    {
        // Gravity applied
        Vector3 gravity = Vector3.down * 9.81f; // Gravity force
        direction = new Vector3(inputVector.x, 0f, inputVector.y);
        //Debug.Log(direction);
        Vector3 movement = direction * speed * Time.deltaTime + gravity * Time.deltaTime;
        return movement;
    }

    public Quaternion CalculateRotation(Vector2 inputVector)
    {
        var facing = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg;
        var turnAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, facing, ref currentVelocityTurn, smoothTurnigTime);
        return Quaternion.Euler(0, turnAngle, 0);
    }

    

}
