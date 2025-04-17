using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using System;

public class PlayerInputHandler : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternativeAction;

    private PlayerConfiguration playerConfig;
    private Player player;
    private PlayerInputActions controls;
    private void Awake()
    {
        controls = new PlayerInputActions();
        controls.Enable();

        player = GetComponent<Player>();

        //controls.Player.Interact.performed += Interact_performed;
        //controls.Player.InteractAlternative.performed += InteractAlternative_performed;
    }
    private void Start()
    {
        OnInteractAction += GameInput_OnInteractAction;
        OnInteractAlternativeAction += GameInput_OnInteractAlternativeAction;

    }
    public void InitializePlayer(PlayerConfiguration pc)
    {
        playerConfig = pc;
        playerConfig.PInput.onActionTriggered += Input_onAccionTriggered;
    }
    private void Input_onAccionTriggered(CallbackContext obj)
    {
        //Debug.Log($"Acción ejecutada: {obj.action.name}");

        if (obj.action.name == controls.Player.Move.name)
        {
            OnMove(obj);
        }
        if (obj.action.name == controls.Player.Interact.name && obj.performed)
        {
            Interact_performed(obj);
            controls.Player.Interact.Disable(); // Deshabilitamos la acción temporalmente
    
        }
        if (obj.action.name == controls.Player.InteractAlternative.name && obj.performed)
        {
            InteractAlternative_performed(obj);
            controls.Player.InteractAlternative.Disable(); // Deshabilitamos la acción temporalmente
        }
    }


    public void OnMove(CallbackContext context)
    {
        if (player != null)
        {
            player.SetInputVector(context.ReadValue<Vector2>());
        }
    }

    private void GameInput_OnInteractAlternativeAction(object sender, EventArgs e)
    {

        // Funcion que se ejecuta con el boton alternativo en player.
        if (player.HasCarObject())
        {
            // Tiramos el objeto.
            player.HandleThrowing();
        }
    }
    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {

        if (player.selectedCounter != null)
        {
            player.selectedCounter.Interact(player);
        }
    }

    public void InteractAlternative_performed(CallbackContext obj)
    {
        Debug.Log("Interact Alternative ejecutado");
        OnInteractAlternativeAction?.Invoke(this, EventArgs.Empty);
    }

    public void Interact_performed(CallbackContext obj)
    {
        Debug.Log("Interact ejecutado");
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
}
