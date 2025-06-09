using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BalacingTool : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
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
    private int spawnLimit; // Limite de ruedas balanceadas que se pueden spawnear.

    private float balancingTimer;

    [SerializeField] private Animator animator;
    private string IS_RUNNING = "Running"; // Nombre del par�metro en el Animator


    public override void Interact(Player player)
    {
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
                // Inicia la animaci�n
                this.AnimationRunning();

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

                // Disparamos el evento para la visual
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
        else
        {
            // There is a car obj here already.
            if (!player.HasCarObject() && spawnLimit >= SpawnLimitManager.Instance.GetSpawnedItemsCount(balancedWheel.name))
            {
                GetCarObject().SetCarObjectParent(player);
                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

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
        spawnLimit = SpawnLimitManager.Instance.GetItemSpawnLimit(balancedWheel.name);

    }

    private void AnimationRunning()
    {
        //Debug.Log("Animator es null.");
        if (animator != null)
        {
            animator.SetTrigger(IS_RUNNING);
            // Aqu� puedes agregar la l�gica que deseas ejecutar cuando la animaci�n de "Running" se complete
            // Por ejemplo, podr�as cambiar el estado del objeto o realizar alguna acci�n adicional.
            //Debug.Log("Animaci�n de 'Running' completada.");
        }
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
                        SpawnLimitManager.Instance.ModifySpawnedCounter(balancedWheel.name, 1);
                        state = State.Done;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

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