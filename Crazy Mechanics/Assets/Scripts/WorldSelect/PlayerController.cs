
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    private Vector2 input;
    private CharacterController playerController;
    private Vector3 direction;

    private Vector3 castDirection;
    [SerializeField] private float speed;
    [SerializeField] private float smoothTurnigTime = 0.5f;
    [SerializeField] LayerMask objectLayerMask;
    [SerializeField] LayerMask levelLayerMask;
    [SerializeField] private Transform holdPoint;
    private float currentVelocityTurn;
    [SerializeField] PauseMenuOverWorld pauseScript;
    [SerializeField] PlayerInput playerInputs;

    private Vector2 inputVector = Vector2.zero;


    private void Awake()
    {
        playerController = GetComponent<CharacterController>();
        //playerInputs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
    }
    void Start()
    {
        InitializePlayer();
    }
    void Update()
    {
        if (!IsWalking()) //If i'm not walking return the update early
        {
            return;
        }

        // calculo de angulo de rotación de personaje
        var facing = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var turnAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, facing, ref currentVelocityTurn, smoothTurnigTime);
        transform.rotation = Quaternion.Euler(0, turnAngle, 0);

        // Gravity applied
        Vector3 gravity = Vector3.down * 9.81f; // Gravity force
        direction = new Vector3(inputVector.x, 0f, inputVector.y);
        Debug.Log(direction);
        Vector3 movement = direction * speed * Time.deltaTime + gravity * Time.deltaTime;

        //Debug.Log($"calculated movement: {movement}");

        // Character movement
        if (playerController != null)
        {
            playerController.Move(movement);
        }
        else
        {
            Debug.LogError("CharacterController unasigned.");
        }
    }

    public void InitializePlayer()
    {
        var playerConfig = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray()[0];
        playerInputs = playerConfig.PInput;
        //playerInputs.SwitchCurrentActionMap("WorldSelect");
        playerInputs.defaultActionMap = "WorldSelect";
        Debug.Log(playerInputs.playerIndex);

        // Configurar los eventos del PlayerInput
        playerInputs.actions["Interact"].performed += Interact;
        playerInputs.actions["Move"].performed += Move_performed;
        playerInputs.actions["Move"].canceled += Move_canceled;
        playerInputs.actions["Pause"].performed += Pause;
    }
    public void SetInputVector(Vector2 direction)
    {
        inputVector = direction;
    }

    /*public void Move(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0.0f, input.y);
        if (direction != Vector3.zero)
        {
            castDirection = direction; // Actualiza donde mira el jugador
        }
    }*/
    private void Move_performed(CallbackContext context)
    {

        SetInputVector(context.ReadValue<Vector2>());
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        // Evita que se ejecute si el botón aún está presionado

        SetInputVector(Vector2.zero);
    }


    public void Pause(InputAction.CallbackContext context)
    {
        pauseScript.TogglePause();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        //Vector3 castDirection = new Vector3(input.x, 0f, input.y);
        //Interaction capsule sizes
        float capsuleDistance = 6f;
        float capsuleRadius = 0.5f;
        float capsuleHeight = 1.5f;

        Vector3 capsuleStart = transform.position;
        Vector3 capsuleEnd = transform.position + Vector3.up * capsuleHeight;

        if (castDirection == Vector3.zero)
        {
            castDirection = direction; //Make facing and interaction direction the same
        }
        /*if(Physics.CapsuleCast(capsuleStart,capsuleEnd, capsuleRadius, direction, out RaycastHit raycastHit, capsuleDistance, objectLayerMask)){//Raycast hitting a interactable object
            Debug.Log("Raycast hit object");
            if(raycastHit.transform.TryGetComponent(out InteractableObject interactable)){
                Debug.Log("calling interact from player controller");
                interactable.Interact();                
            }
        }*/
        Debug.Log("where " + castDirection);
        if (Physics.CapsuleCast(capsuleStart, capsuleEnd, capsuleRadius, castDirection, out RaycastHit raycastHit, capsuleDistance, levelLayerMask))
        {//Raycast hiting a level select object
            //Debug.Log("Raycast hit world" + raycastHit.transform.name);
            if (raycastHit.transform.TryGetComponent(out WorldObject worldObject))
            {
                //Debug.Log("calling world from player controller");
                worldObject.Interact();
            }
        }
    }

    public bool IsWalking()
    {
        //Check if the character is currently moving
        //Debug.Log(inputVector.sqrMagnitude);
        Debug.Log(inputVector);
        return inputVector != Vector2.zero;

    }
}
