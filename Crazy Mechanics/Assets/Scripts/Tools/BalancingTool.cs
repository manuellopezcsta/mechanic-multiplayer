using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalacingTool : BaseCounter, IHasProgress
{

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    private enum State
    {
        Idle,
        Running,
        Done,
    }

    private State state;
    [SerializeField] private ObjectsSO wheel;
    [SerializeField] private ObjectsSO balancedWheel;
    [SerializeField] private bool taskComplete;
    [SerializeField] private float balancingTimerMax;

    private float balancingTimer;



    public override void Interact(Player player)
    {
        //Debug.Log("Se entro aca");
        // Logica para dejar objetos
        if (!HasCarObject())
        {
            // There is no obj here and check if they are the same object
            if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == wheel && GameManager.Instance.IsPowerEnabled())
            {
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
                state = State.Running;
                balancingTimer = 0f;

                // Disparamos el evento para la visual
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });

                //StartCoroutine(TimeToRequest(timeRequest));
            }
        }
        else
        {
            // There is a car obj here already.
            if (!player.HasCarObject())
            {
                GetCarObject().SetCarObjectParent(player);
                state = State.Idle;

                // Disparamos el evento para la visual
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0
                });
            }
        }
    }
    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        // Logica del Timer para cambiar de rueda
        if (HasCarObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Running:
                    balancingTimer += Time.deltaTime;

                    // Disparamos el evento para la visual
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = balancingTimer / balancingTimerMax
                    });

                    // Cuando se cumpla el tiempo.
                    if (balancingTimer > balancingTimerMax)
                    {
                        // Destruyo la vieja y coloco la nueva.
                        GetCarObject().DestroySelf();
                        CarObject.SpawnKitchenObject(balancedWheel, this);
                        state = State.Done;
                        Debug.Log("Rueda Inflada!");
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
