using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 input;
    private CharacterController  playerController;
    private PlayerInput playerInput;
    private Vector3 direction;
    [SerializeField] private float speed;
    [SerializeField] private float smoothTurnigTime = 0.5f;
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
        Debug.Log("interaction");
    }
}
