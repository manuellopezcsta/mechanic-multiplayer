using System;
using UnityEngine;

//Player scrip for world select scene
public class PlayerWorldSelect : MonoBehaviour//, ICarObjectParent
{
    public static PlayerWorldSelect Instance { get; private set; }
    public static event EventHandler OnPickedSomething;
    public static event EventHandler OnDroppedSomething;

    //[SerializeField] private GameInput gameInput;
    [SerializeField] private Transform handPosition;
    [SerializeField] private Transform invisbleHolderPosition;
    
    //Throw power variables
    [SerializeField] float throwX;
    [SerializeField] float throwY;
    [SerializeField] float throwZ;

    private OverworldInteractable interactableObject;

    private bool actionWalking = false;

    private Vector3 playerFacing;

    private CharacterController characterController;
    
    private void Awake(){
        Instance = this;//on awake create an instance
    }

    private void Start() {
        characterController = GetComponent<CharacterController>();

        //gameInput.OnInteractAction += GameInput_Interact;
        
    }

    private void GameInput_Interact(object sender, EventArgs e) {
        if(interactableObject != null){
            interactableObject.Interact(this);//If I'm not already holding something interact with nerby interactable object
        }
    }

    private void Update()
    {
        
    }



    public bool IsWalking(){ //Returns if the player is currently walking used for animations
        return actionWalking;
    }




    private void DebugInteractionCapsule(bool active, Vector3  capsuleStart, Vector3 capsuleEnd, float interactReach, float interactRadius) {
        Vector3 offset = playerFacing.normalized * interactReach;

        if(active) {
            // Dibujar la c�psula en la escena
        Debug.DrawLine(capsuleStart, capsuleEnd, Color.red);
        Debug.DrawLine(capsuleStart + offset, capsuleEnd + offset, Color.red);
        Debug.DrawLine(capsuleStart, capsuleStart + offset, Color.red);
        Debug.DrawLine(capsuleEnd, capsuleEnd + offset, Color.red);
        Debug.DrawRay(capsuleStart, Vector3.up * interactRadius, Color.red);
        Debug.DrawRay(capsuleEnd, Vector3.up * interactRadius, Color.red);
        Debug.DrawRay(capsuleStart + offset, Vector3.up * interactRadius, Color.red);
        Debug.DrawRay(capsuleEnd + offset, Vector3.up * interactRadius, Color.red);
        }
    }
}