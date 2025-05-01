using System.Collections;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public bool activado = false;

    public Vector3 windToTheRight = new Vector3(1, 0, 0); //viento a la derecha
    public Vector3 windToTheLeft = new Vector3(-1, 0, 0); // viendo a la izquierda
    public Vector3 windDirection;

    public int timeToChangeDirection;
    public float windForce = 5f; // Intensidad del viento

    // Listado de etiquetas afectadas por el viento
    public string flyObjectsTag;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(ChangeDirectionWind());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        
        if (activado)
        {
            // Verificar si el objeto tiene una etiqueta que debe ser afectada
            if (flyObjectsTag.Contains(other.gameObject.tag))
            {
                //Debug.Log(other.name);
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Aplicar fuerza en la dirección del viento
                    rb.AddForce(windDirection * windForce, ForceMode.Acceleration);
                }
            }
        }
    }

    IEnumerator ChangeDirectionWind()
    {
        if (!activado)
        {
            activado = !activado;
            windDirection = windToTheRight;
            yield return new WaitForSeconds(timeToChangeDirection);
            windDirection = windToTheLeft;
            yield return new WaitForSeconds(timeToChangeDirection);
            activado = !activado;
        }
    }
}