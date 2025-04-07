using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class CarController : MonoBehaviour
{
    private CurrentStationManager currentStationManager;

    public bool taskComplete = true;
    public bool canMove;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float targetZ = 10f;

    void Update()
    {
        if (canMove && taskComplete)
        {
            // El auto va hacia adelante.
            Vector3 currentPosition = transform.position;
            float newZ = Mathf.MoveTowards(currentPosition.z, targetZ, speed * Time.deltaTime);
            transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);
        }
        if (transform.position.z <= targetZ)
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentStationManager(CurrentStationManager target) {
        currentStationManager = target;
    }

    public CurrentStationManager GetCurrentStationManager() {
        return currentStationManager;
    }

    // El car controller crea las tasks del auto.

    // Cuando las crea , asigno a la task si hace falta una ref al car controller.
    // task.carController = this;
}
