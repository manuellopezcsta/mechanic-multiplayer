using System.Collections;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public bool isActive = false;

    // Listado de etiquetas afectadas por el viento
    //const string AFFECTED_OBJECTS_TAG = "Wind";
    private Vector3 windDirection;
    [SerializeField] int windDuration = 2;
    [SerializeField] float windForce = 18f; // Intensidad del viento

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(ChangeWindDirection());
        }
    }*/

    // Disparador del evento.
    public static WindManager Instance {get; private set;}
    void Awake()
    {
        Instance = this;
    }

    public void TriggerWindEvent() {
        StartCoroutine(ChangeWindDirection());
    }
    private void OnTriggerStay(Collider other)
    {
        if (isActive)
        {
            // Verificar si el objeto tiene una etiqueta que debe ser afectada
            //if (other.gameObject.CompareTag(AFFECTED_OBJECTS_TAG))

            // Nuevo metodo verifica si es un objeto en el piso o la pluma sin tag.
            if(other.TryGetComponent(out InvisibleHolder invisibleHolder) || other.TryGetComponent(out MotorTool motorTool))
            {
                //Debug.Log(other.name);
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Aplicar fuerza en la direccion del viento
                    rb.AddForce(windDirection * windForce, ForceMode.Acceleration);
                }
            }
        }
    }

    IEnumerator ChangeWindDirection()
    {
        if (!isActive)
        {
            isActive = !isActive;
            // Elegimos una direccion random para el viento
            windDirection = ChooseRandomWindDirection();
            yield return new WaitForSeconds(windDuration);
            // Invertimos el viento
            windDirection *= -1;
            yield return new WaitForSeconds(windDuration);
            isActive = !isActive;
            // Apagamos el Desastre.
            DisasterManager.Instance.disasterHappening = false;
        }
    }

    Vector3 ChooseRandomWindDirection() {
        int random = Random.Range(0, 4);
        switch (random) {
            case 0:
                return new Vector3(1, 0, 0);
            case 1:
                return new Vector3(-1, 0, 0);
            case 2:
                return new Vector3(1, 0, 1);
            case 3:
                return new Vector3(0, 0, -1);
            default:
                return Vector3.zero;
        }
    }
}