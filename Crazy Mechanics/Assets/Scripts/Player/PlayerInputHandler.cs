using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;

public class PlayerInputHandler : MonoBehaviour
{
    // Script que configura la toma de inputs del player, al joystick correspondiente.
    private PlayerConfiguration playerConfig;
    private PlayerInput playerInput;
    private Player player;
    private PlayerMovement playerMovement;
    private PlayerInteract playerInteract;
    const string WALL_TAG = "Pared";


    [SerializeField] PlayerAnimator playerAnimator;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInteract = GetComponent<PlayerInteract>();
        //Debug.Log($"PlayerInputHandler creado: {gameObject.name}");
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
        Debug.Log(playerInput.playerIndex);

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
        playerMovement.Dash();
    }
    private void Pause_perfomed(CallbackContext context)
    {
        PauseMenuScriptUI pauseMenuScript = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<PauseMenuScriptUI>();
        //Debug.Log(pauseMenuScript.CanExitPause());

        if (pauseMenuScript != null && pauseMenuScript.CanExitPause())
        {
            pauseMenuScript.TogglePause();
        }
    }

    private void Move_performed(CallbackContext context)
    {
        if (player != null)
        {
            playerMovement.SetInputVector(context.ReadValue<Vector2>());
        }
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        if (player != null)
        {
            // Cuando se cancela el movimiento, establecer el vector a (0, 0)
            playerMovement.SetInputVector(Vector2.zero);
        }
    }


    private void InteractAlternative_performed(CallbackContext context)
    {
        // Funcion que se ejecuta con el boton alternativo en player.
        if (player.HasCarObject())
        {
            // Tiramos el objeto.
            playerInteract.HandleThrowing();
        }
        else
        {
            //Ejecutamos la funcion de baile cuando el player no tiene nada en la mano
            playerAnimator.StartDance();
        }
    }

    private void Interact_performed(CallbackContext context)
    {
        /*if (player.selectedCounter != null)
        {
            player.selectedCounter.Interact(player);
        }*/

        // Obtiene todos los objetos en rango y intenta interactuar con ellos
        List<RaycastHit> hits = new List<RaycastHit>();
        foreach (RaycastHit hit in playerInteract.GetAllObjectInRange())
        {
            hits.Add(hit);

        }
        hits = hits.OrderBy(hit => hit.distance).ToList();
        if (hits.Count == 0)
        {
            return;
        }
        else if (hits[0].collider.gameObject.CompareTag(WALL_TAG))
        {
            return;
        }
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.CompareTag(WALL_TAG))
            {
                continue;
            }
            hit.collider.GetComponent<BaseCounter>().Interact(player);
        }

    }
}
