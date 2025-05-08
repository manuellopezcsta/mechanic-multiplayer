using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 input;
    private CharacterController  playerController;
    private Vector3 direction;

    private Vector3 castDirection;
    [SerializeField] private float speed;
    [SerializeField] private float smoothTurnigTime = 0.5f;
    [SerializeField] LayerMask objectLayerMask;
    [SerializeField] LayerMask levelLayerMask;
    [SerializeField] private Transform holdPoint;
    private float currentVelocityTurn;
    [SerializeField] PauseMenuOverWorld pauseScript;
    [SerializeField] InputActionAsset playerInputs;


    private void Awake()
    {
        playerController = GetComponent<CharacterController>();
        //playerInputs.Enable();
    }

    void Update()
    {
        if (! IsWalking()) //If i'm not walking return the update early
        {
            return;
        }

        // calculo de angulo de rotación de personaje
        var facing = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var turnAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, facing, ref currentVelocityTurn, smoothTurnigTime);
        transform.rotation = Quaternion.Euler(0, turnAngle, 0);

        // Gravity applied
        Vector3 gravity = Vector3.down * 9.81f; // Gravity force
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

    public void Move(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0.0f, input.y);
        if(direction !=Vector3.zero){
            castDirection = direction; // Actualiza donde mira el jugador
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        pauseScript.TogglePause();
    }

    public void Interact (InputAction.CallbackContext context) {
        //Vector3 castDirection = new Vector3(input.x, 0f, input.y);
        //Interaction capsule sizes
        float capsuleDistance = 6f;
        float capsuleRadius = 0.5f;
        float capsuleHeight = 1.5f;

        Vector3 capsuleStart = transform.position;
        Vector3 capsuleEnd = transform.position + Vector3.up*capsuleHeight;

        if(castDirection == Vector3.zero){
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
        if(Physics.CapsuleCast(capsuleStart,capsuleEnd, capsuleRadius, castDirection, out RaycastHit raycastHit, capsuleDistance, levelLayerMask)){//Raycast hiting a level select object
            //Debug.Log("Raycast hit world" + raycastHit.transform.name);
            if(raycastHit.transform.TryGetComponent(out WorldObject worldObject)){
                //Debug.Log("calling world from player controller");
                worldObject.Interact();                
            }    
        }
    }

    public bool IsWalking(){ //Check if the character is currently moving
        return input.sqrMagnitude !=0;
    }
}
