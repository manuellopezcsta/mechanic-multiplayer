using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class WindManager : MonoBehaviour
{
    public bool isActive = false;

    // Listado de etiquetas afectadas por el viento
    //const string AFFECTED_OBJECTS_TAG = "Wind";
    private Vector3 windDirection;
    [SerializeField] int windDuration = 2;
    [SerializeField] float windForce = 18f; // Intensidad del viento
    [SerializeField] private GameObject[] fxWind; //Orden de fxWind ( West, East, South, North)
    private String directionName;

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
            //windDirection *= -1;
            ChangeDirectionWind();
            yield return new WaitForSeconds(windDuration);
            TurnOffFx();
            isActive = !isActive;
            // Apagamos el Desastre.
            DisasterManager.Instance.disasterHappening = false;
        }
    }

    Vector3 ChooseRandomWindDirection() {
        int random = UnityEngine.Random.Range(0, 4);
        switch (random) {
            case 0:
                //West
                fxWind[0].SetActive(true);
                directionName = "West";
                //Debug.Log("0");
                return new Vector3(1, 0, 0);
            case 1:
                //East
                fxWind[1].SetActive(true);
                directionName = "East";
                //Debug.Log("1");
                return new Vector3(-1, 0, 0);
            case 2:
                //North
                fxWind[2].SetActive(true);
                directionName = "North";
                //Debug.Log("2");
                return new Vector3(0, 0, 1);
            case 3:
                //South
                fxWind[3].SetActive(true);
                directionName = "South";
                //Debug.Log("3");
                return new Vector3(0, 0, -1);
            default:
                return Vector3.zero;
        }
    }
    private void ChangeDirectionWind(){
        switch(directionName){
            case "West":
            fxWind[0].SetActive(false);
            fxWind[1].SetActive(true);
            break;
            case "East":
            fxWind[1].SetActive(false);
            fxWind[0].SetActive(true);
            break;
            case "North":
            fxWind[2].SetActive(false);
            fxWind[3].SetActive(true);
            break;
            case "South":
            fxWind[3].SetActive(false);
            fxWind[2].SetActive(true);
            break;
        }
        windDirection *= -1;
    }
    private void TurnOffFx(){
        foreach(GameObject fx in fxWind){
            fx.SetActive(false);
        }
    }
}