using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerInputHandler : MonoBehaviour
{
    // Script que configura la toma de inputs del player, al joystick correspondiente.
    private PlayerConfiguration playerConfig;
    private PlayerInput playerInput;
    private Player player;
    private PlayerMovement playerMovement;
    private PlayerInteract playerInteract;


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

        if (pauseMenuScript != null)
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

        /*foreach (RaycastHit hit in playerInteract.GetAllObjectInRange())

        {
            hit.collider.GetComponent<BaseCounter>().Interact(player);
        }*/
        RaycastHit? primerHit = playerInteract.GetFirstInteractableObject();

        if (primerHit.HasValue)
        {
            if (primerHit.Value.collider.TryGetComponent<BaseCounter>(out BaseCounter baseCounter))
            {
                baseCounter.Interact(player);
            }
        }


    }
}
