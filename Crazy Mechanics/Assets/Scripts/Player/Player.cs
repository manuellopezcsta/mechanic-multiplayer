using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Player : MonoBehaviour, ICarObjectParent
{
    const string MOTOR_TOOL_NAME = "Pluma";

    public static event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public static event EventHandler OnPickedSomething;
    public static event EventHandler OnDroppedSomething;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform carObjectHoldPoint;
    [SerializeField] private Transform invisibleHolder;

    // Para tirar
    [SerializeField] float throwMagnitude;
    [SerializeField] float fowardMagnitude;
    [SerializeField] float upMagnitude;


    private bool isWalking = false;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private CarObject carObject;

    // Char controller y collider para 2do metodo de movimiento.
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternativeAction += GameInput_OnInteractAlternativeAction;
    }

    private void GameInput_OnInteractAlternativeAction(object sender, EventArgs e)
    {
        // Funcion que se ejecuta con el boton alternativo en player.
        if (HasCarObject())
        {
            // Tiramos el objeto.
            HandleThrowing();
        }
    }

    private void HandleThrowing()
    {
        // Me desligo del objeto
        // Creamos un nuevo padre para el obj
        Transform holder = Instantiate(invisibleHolder);
        InvisibleHolder holderCounter = holder.GetComponent<InvisibleHolder>();
        // Arreglamos la pos y rotacion
        holder.position = GetCarObjectFollowTransform().position;
        holder.rotation = transform.rotation;

        carObject.SetCarObjectParent(holderCounter);
        // Arreglo el tama;o del collider
        holderCounter.FixColliderSize();
        // Arreglo su visual
        holder.GetComponentInChildren<SelectedVisualInvisible>().SetUpSelected();
        // Lo tiro
        Debug.Log("Se Tiro");
        Vector3 forceDirection = ((transform.forward * fowardMagnitude) + Vector3.up * upMagnitude) * throwMagnitude;
        holderCounter.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
        // Reproducimos el sonidito
        OnDroppedSomething?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleMovement(){
        // Capturamos al vector desde GameImput y se lo aplicamos al char controller.
        float speed = 7f;
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        //SimpleMove no ignora la gravedad.
        characterController.Move(moveDir * Time.deltaTime * speed);
        // Para arreglar un bug donde flota el player?
        characterController.Move(Vector3.down * Time.deltaTime * speed);
        //transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        
        

        // Rotamos al personaje.
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

        // Actualizamos el booleano para la animacion.
        isWalking = moveDir != Vector3.zero;

    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        float capsuleRadius = 0.5f; // Ajusta el radio seg�n sea necesario
        float capsuleHeight = 1.5f; // Ajusta la altura seg�n sea necesario

        Vector3 capsuleStart = transform.position;
        Vector3 capsuleEnd = transform.position + Vector3.up * capsuleHeight;

        // If we hit something
        DebugInteractionCapsule(true, capsuleStart, capsuleEnd, interactDistance, capsuleRadius);
        
        //if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        if (Physics.CapsuleCast(capsuleStart, capsuleEnd, capsuleRadius, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            //Debug.Log(raycastHit.transform.name);
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Debug.Log(raycastHit.transform.name);
                // Has clear Counter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void DebugInteractionCapsule(bool active, Vector3  capsuleStart, Vector3 capsuleEnd, float interactDistance, float capsuleRadius) {
        Vector3 offset = lastInteractDir.normalized * interactDistance;

        if(active) {
            // Dibujar la c�psula en la escena
        Debug.DrawLine(capsuleStart, capsuleEnd, Color.red);
        Debug.DrawLine(capsuleStart + offset, capsuleEnd + offset, Color.red);
        Debug.DrawLine(capsuleStart, capsuleStart + offset, Color.red);
        Debug.DrawLine(capsuleEnd, capsuleEnd + offset, Color.red);
        Debug.DrawRay(capsuleStart, Vector3.up * capsuleRadius, Color.red);
        Debug.DrawRay(capsuleEnd, Vector3.up * capsuleRadius, Color.red);
        Debug.DrawRay(capsuleStart + offset, Vector3.up * capsuleRadius, Color.red);
        Debug.DrawRay(capsuleEnd + offset, Vector3.up * capsuleRadius, Color.red);
        }
    }

    // Para empujar la pluma
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        float forceMagnitude = 1f;

        if (rb != null  && hit.gameObject.name == MOTOR_TOOL_NAME) {
            Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
            forceDirection.y = 0;
            forceDirection.Normalize();

            rb.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
        }
    }
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetCarObjectFollowTransform()
    {
        return carObjectHoldPoint;
    }

    public void SetCarObject(CarObject target)
    {
        this.carObject = target;

        if(this.carObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public CarObject GetCarObject()
    {
        return carObject;
    }

    public void ClearCarObject()
    {
        carObject = null;
    }

    public bool HasCarObject()
    {
        return carObject != null;
    }
}