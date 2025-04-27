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

    public static event EventHandler OnFixingLightBox;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;


    public void CutDownPower() {
        isPowerDown = true;
        fixingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
            progressNormalized = (float) fixingProgress / fixingProgressMax
        });
        // Alguna animacion x aca ? ..
    }

    public bool IsPowerDown() {
        return isPowerDown;
    }

    public override void Interact(Player player)
    {
        // Si el player esta holdeando la fixing tool
        if(player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool && isPowerDown) {
            fixingProgress ++;

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
            progressNormalized = (float) fixingProgress / fixingProgressMax
        });

            if(fixingProgress >= fixingProgressMax) {
                isPowerDown = false;
                player.GetCarObject().DestroySelf();
                Debug.Log("Luz Arreglada");
            }
        }
    }
}
