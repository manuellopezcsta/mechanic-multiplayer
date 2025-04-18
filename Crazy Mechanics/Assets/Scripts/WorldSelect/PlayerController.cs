using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 input;
    private CharacterController  playerController;
    private Vector3 direction;
    [SerializeField] private float speed;
    [SerializeField] private float smoothTurnigTime = 0.5f;
    [SerializeField] LayerMask objectLayerMask;
    [SerializeField] LayerMask levelLayerMask;
    [SerializeField] private Transform holdPoint;
    private float currentVelocityTurn;


    private void Awake()
    {
        playerController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    void Update()
    {   
        if(input.sqrMagnitude == 0) return;
        var facing = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg; //Metemathicalli calculates the angle that the character should be facing
        var turnAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, facing, ref currentVelocityTurn, smoothTurnigTime); //Calculate smooth rotation by frame
        transform  .rotation = Quaternion.Euler(0,turnAngle,0);
        playerController.Move(direction * speed * Time.deltaTime);
    }
    public void Move (InputAction.CallbackContext context) {
    
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0.0f, input.y);
    
    }

    public void Interact (InputAction.CallbackContext context) {
        Vector3 castDirection = new Vector3(input.x, 0f, input.y);
        //Interaction capsule sizes
        float capsuleDistance = 2f;
        float capsuleRadius = 0.5f;
        float capsuleHeight = 1.5f;

        Vector3 capsuleStart = transform.position;
        Vector3 capsuleEnd = transform.position + Vector3.up*capsuleHeight;

        if(castDirection != Vector3.zero){
            direction = castDirection; //Make facing and interaction direction the same
        }
        if(Physics.CapsuleCast(capsuleStart,capsuleEnd, capsuleRadius, direction, out RaycastHit raycastHit, capsuleDistance, objectLayerMask)){//Raycast hitting a interactable object
            Debug.Log("Raycast hit object");
            if(raycastHit.transform.TryGetComponent(out InteractableObject interactable)){
                Debug.Log("calling interact from player controller");
                interactable.Interact();                
            }
        }
        if(Physics.CapsuleCast(capsuleStart,capsuleEnd, capsuleRadius, direction, out raycastHit, capsuleDistance, levelLayerMask)){//Raycast hiting a level select object
            Debug.Log("Raycast hit world" + raycastHit.transform.name);
            if(raycastHit.transform.TryGetComponent(out WorldObject worldObject)){
                Debug.Log("calling world from player controller");
                worldObject.Interact();                
            }    
        }
    }
}
