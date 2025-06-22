using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBoxController : BaseCounter, IHasProgress
{
    [SerializeField] ObjectsSO fixingTool;
    [SerializeField] bool isPowerDown = false;

    // Para la barra de progreso
    private int fixingProgress;
    [SerializeField] private int fixingProgressMax;
    [SerializeField] private GameObject fxElectricity;
    public static event EventHandler OnFixingLightBox;
    public static event EventHandler OnLightShutdown;
    public static event EventHandler OnLightTurnOn;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;


    public void CutDownPower() {
        isPowerDown = true;
        fxElectricity.SetActive(true);
        fixingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
            progressNormalized = (float) fixingProgress / fixingProgressMax
        });
        // Alguna animacion x aca ? ..

        OnLightShutdown?.Invoke(this, EventArgs.Empty);
        Debug.Log("Se corto la luz..");
    }

    public bool IsPowerDown() {
        return isPowerDown;
    }

    public override void Interact(Player player)
    {
        // Si el player esta holdeando la fixing tool
        if(player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool && isPowerDown) {
            fixingProgress ++;
            // Disparamos el evento de ruido
            OnFixingLightBox?.Invoke(this, EventArgs.Empty);

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
            progressNormalized = (float) fixingProgress / fixingProgressMax
        });

            if(fixingProgress >= fixingProgressMax) {
                isPowerDown = false;
                OnLightTurnOn?.Invoke(this, EventArgs.Empty);
                fxElectricity.SetActive(false);
                player.GetCarObject().DestroySelf();
                DisasterManager.Instance.disasterHappening = false;
                Debug.Log("Luz Arreglada");
            }
        }
    }
}
