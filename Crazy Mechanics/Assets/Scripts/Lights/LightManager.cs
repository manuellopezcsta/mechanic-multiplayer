using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

public class LightManager : MonoBehaviour
{
    [Header("Settigs Lighting Event")]
    [SerializeField] private Light[] discoLights;
    [SerializeField] private float minTiempo = 0.5f; // Tiempo minimo antes de cambiar de color
    [SerializeField] private float maxTiempo = 2f;  // Tiempo máximo para cambiar de color

    public bool isActive;
    [SerializeField] private int eventTime;

    [SerializeField] private GameObject lightDefault;
    [SerializeField] private GameObject lightDisco;


    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerDiscoNight();
        }*/
    }

    private void ChangeEventLights()
    {
        isActive = !isActive;
        lightDisco.SetActive(isActive);
        lightDefault.SetActive(!isActive);
        if (isActive)
        {
            TurnOnLights();
            RenderSettings.ambientLight = Color.black;
        }
        else
        {
            RenderSettings.ambientLight = Color.white;
        }
    }

    private void TurnOnLights()
    {
        foreach (Light luces in discoLights)
        {
            StartCoroutine(ChangeTheLightColor(luces));
        }
    }
    IEnumerator ChangeTheLightColor(Light luz)
    {
        while (isActive)
        {
            yield return new WaitForSeconds(Random.Range(minTiempo, maxTiempo)); // Espera un tiempo aleatorio entre el minimo y el maximo
            luz.color = new Color(Random.value, Random.value, Random.value); // Cambia de color a un random
        }
    }

    IEnumerator ActivateDiscoNight()
    {
        if (!isActive)
        {
            ChangeEventLights();
            yield return new WaitForSeconds(eventTime);
            ChangeEventLights();
            DisasterManager.Instance.disasterHappening = false;
        }
    }
    
    public void TriggerDiscoNight ()
    {
        StartCoroutine(ActivateDiscoNight());
    }
}
