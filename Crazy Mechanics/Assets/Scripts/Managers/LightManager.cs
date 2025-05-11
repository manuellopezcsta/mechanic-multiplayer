using System.Collections;
using UnityEngine;


public class LightManager : MonoBehaviour
{
    [Header("Event Settings")]
    [SerializeField] private Light[] discoLights;
    [SerializeField] private float minTime = 0.2f; // Tiempo minimo antes de cambiar de color
    [SerializeField] private float maxTime = 1f;  // Tiempo m�ximo para cambiar de color

    public bool isActive;
    [SerializeField] private int eventTime;

    [SerializeField] private GameObject defaultLight;
    [SerializeField] private GameObject discoLight;
    [SerializeField] Material defaultSkybox;


    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerDiscoNight();
        }
    }*/

    private void ChangeEventLights()
    {
        isActive = !isActive;
        discoLight.SetActive(isActive);
        defaultLight.SetActive(!isActive);
        if (isActive)
        {
            TurnOnLights();
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            RenderSettings.ambientLight = Color.black;
        }
        else
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
            RenderSettings.ambientLight = Color.white;
        }
    }

    private void TurnOnLights()
    {
        foreach (Light luces in discoLights)
        {
            StartCoroutine(RandomizeLightColor(luces));
        }
    }

    IEnumerator RandomizeLightColor(Light luz)
    {
        while (isActive)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime)); // Espera un tiempo aleatorio entre el minimo y el maximo
            luz.color = new Color(Random.value, Random.value, Random.value); // Cambia de color a un random
        }
    }

    IEnumerator ActivateDiscoNight()
    {
        if (!isActive)
        {
            ChangeEventLights();
            yield return new WaitForSeconds(eventTime);
            ChangeSkyBoxLighting();
            ChangeEventLights();
            DisasterManager.Instance.disasterHappening = false;
        }
    }

    // Dispara los eventos para iniciar el disaster
    public void TriggerDiscoNight()
    {
        ChangeSkyBoxLighting();
        StartCoroutine(ActivateDiscoNight());
    }

    // Cambia la skybox para que la iluminacion sea mejor durante el evento.
    void ChangeSkyBoxLighting()
    {
        if (!isActive)
        {
            RenderSettings.skybox = null;
        }
        else
        {
            RenderSettings.skybox = defaultSkybox;
        }
    }
}
