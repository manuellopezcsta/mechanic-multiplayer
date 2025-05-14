using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorTool : BaseCounter, IHasProgress
{
    [SerializeField] private ObjectsSO battery;

    [SerializeField] private ObjectsSO chargeBattery;
    [SerializeField] private float chargeTimerMax;
    private float chargeTimer;
    [SerializeField] private GameObject fxCharging;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    private enum State
    {
        Idle,
        Running,
        Done,
    }

    private State state;
    

    public override void Interact(Player player)
    {

        // Logica para dejar objetos
        if (!HasCarObject()) {
            // There is no obj here and check if they are the same object
            // SI lo pones y justo se corta la luz carga igual..
            if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == battery && GameManager.Instance.IsPowerEnabled()) {
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
                fxCharging.SetActive(true);
                chargeTimer = 0f;
                state = State.Running;
            }
        } else {
            // There is a car obj here already.
            if(!player.HasCarObject()) {
                GetCarObject().SetCarObjectParent(player);
                chargeTimer = 0f;
                state = State.Idle;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
            } 
        }
    }

    private void Update() {
        // Logica del Timer para cambiar de rueda
        if (HasCarObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Running:
                    chargeTimer += Time.deltaTime;

                    // Disparamos el evento para la visual
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = chargeTimer / chargeTimerMax
                    });

                    // Cuando se cumpla el tiempo.
                    if (chargeTimer > chargeTimerMax)
                    {
                        // Destruyo la vieja y coloco la nueva.
                        GetCarObject().DestroySelf();
                        CarObject.SpawnKitchenObject(chargeBattery, this);
                        state = State.Done;
                        Debug.Log("Bateria Cargada!");
                        fxCharging.SetActive(false);
                    }
                    break;
                case State.Done:
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                    break;
            }
        }
    }
}
