using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using System;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerInputHandler : MonoBehaviour
{
    // Script que configura la toma de inputs del player, al joystick correspondiente.
    private PlayerConfiguration playerConfig;
    private PlayerInput playerInput;
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
        Debug.Log($"PlayerInputHandler creado: {gameObject.name}");
    }

    public void InitializePlayer(PlayerConfiguration pc)
    {
        if (playerConfig != null)
        {
            Debug.LogWarning("Player already initialized!");
            return;
        }

        playerConfig = pc;
        playerInput = playerConfig.PInput;

        // Configurar los eventos del PlayerInput
        playerInput.actions["Interact"].performed += Interact_performed;
        playerInput.actions["InteractAlternative"].performed += InteractAlternative_performed;
        playerInput.actions["Move"].performed += Move_performed;
        playerInput.actions["Move"].canceled += Move_canceled; // Para cancelar el movimiento.
        playerInput.actions["Pause"].performed += Pause_perfomed;
        playerInput.actions["Dash"].performed += Dash_performed;
    }


    public void UnsuscribeController()
    {
        // Ver si siguen estando los problemas de doble trigger,
        // cuando se termina la partida, y volves al world select y empieza otro.
        // Fijarse de usarlo en casos de ir al menu principal !!


        // CODIGO PARA DESUSCRIBIRSE AL DESTRUIRSE UN PLAYER.
        if (playerInput != null)
        {
            playerInput.actions["Interact"].performed -= Interact_performed;
            playerInput.actions["InteractAlternative"].performed -= InteractAlternative_performed;
            playerInput.actions["Move"].performed -= Move_performed;
            playerInput.actions["Move"].canceled -= Move_canceled;
            playerInput.actions["Pause"].performed -= Pause_perfomed;
            playerInput.actions["Dash"].performed -= Dash_performed;
        }
    }

    private void Dash_performed(CallbackContext context)
    {
        player.Dash();
    }
    private void Pause_perfomed(CallbackContext context)
    {
        PauseMenuScriptUI pauseMenuScript = GameObject.Find("Canvas").GetComponent<PauseMenuScriptUI>();
        
        if (pauseMenuScript != null)
        {
            pauseMenuScript.TogglePause();
        }
    }

    private void Move_performed(CallbackContext context)
    {
        if (player != null)
        {
            player.SetInputVector(context.ReadValue<Vector2>());
        }
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        if (player != null)
        {
            // Cuando se cancela el movimiento, establecer el vector a (0, 0)
            player.SetInputVector(Vector2.zero);
        }
    }


    private void InteractAlternative_performed(CallbackContext context)
    {
        // Funcion que se ejecuta con el boton alternativo en player.
        if (player.HasCarObject())
        {
            // Tiramos el objeto.
            player.HandleThrowing();
        }
    }

    private void Interact_performed(CallbackContext context)
    {
        if (player.selectedCounter != null)
        {
            player.selectedCounter.Interact(player);
        }
    }
}
