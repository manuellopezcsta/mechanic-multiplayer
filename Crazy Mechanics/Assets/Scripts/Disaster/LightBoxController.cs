using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBoxController : BaseCounter, IHasProgress
{
    [SerializeField] Material defaultSkybox;
    [SerializeField] private GameObject defaultLight;
    [SerializeField] ObjectsSO fixingTool;
    [SerializeField] bool isPowerDown = false;

    [SerializeField] GameObject[] lights;
    [SerializeField] GameObject[] objectsToTurnOn;
    [SerializeField] GameObject[] powerLights;
    [SerializeField] Material[] materialLights;
    // Para la barra de progreso
    private int fixingProgress;
    [SerializeField] private int fixingProgressMax;
    [SerializeField] private GameObject fxElectricity;
    public static event EventHandler OnFixingLightBox;
    public static event EventHandler OnLightShutdown;
    public static event EventHandler OnLightTurnOn;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;


    public void CutDownPower()
    {
        isPowerDown = true;
        ChangeLights();
        fxElectricity.SetActive(true);
        fixingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)fixingProgress / fixingProgressMax
        });
        // Alguna animacion x aca ? ..

        OnLightShutdown?.Invoke(this, EventArgs.Empty);
        Debug.Log("Se corto la luz..");
    }

    public bool IsPowerDown()
    {
        return isPowerDown;
    }

    public override void Interact(Player player)
    {
        // Si el player esta holdeando la fixing tool
        if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool && isPowerDown)
        {
            fixingProgress++;
            // Disparamos el evento de ruido
            OnFixingLightBox?.Invoke(this, EventArgs.Empty);

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)fixingProgress / fixingProgressMax
            });

            if (fixingProgress >= fixingProgressMax)
            {
                isPowerDown = false;
                OnLightTurnOn?.Invoke(this, EventArgs.Empty);
                fxElectricity.SetActive(false);
                player.GetCarObject().DestroySelf();
                DisasterManager.Instance.disasterHappening = false;
                ChangeLights();
                Debug.Log("Luz Arreglada");
            }
        }
    }

    private void ChangeLights()
    {
        defaultLight.SetActive(!isPowerDown);

        if (isPowerDown)
        {
            TurnOff(materialLights[1],isPowerDown);
            TurnOffFases(materialLights[2]);
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            RenderSettings.ambientLight = Color.black;
        }
        else
        {
            TurnOff(materialLights[0],isPowerDown);
            TurnOffFases(materialLights[3]);
            RenderSettings.skybox = defaultSkybox;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
            RenderSettings.ambientLight = Color.white;
        }
    }

    private void TurnOff(Material value, bool changeState)
    {
        foreach (GameObject obj in objectsToTurnOn)
        {
            obj.SetActive(changeState);
        }
        foreach (GameObject obj in lights)
        {
            //obj.SetActive(value);
            obj.GetComponent<MeshRenderer>().material = value;
        } 
    }
    private void TurnOffFases(Material value)
    {
       foreach (GameObject obj in powerLights)
        {
            //obj.SetActive(value);
            obj.GetComponent<MeshRenderer>().material = value;
        } 
    }
}
