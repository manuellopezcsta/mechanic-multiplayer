using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICarObjectParent
{

    public static Player Instance { get; private set; }
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] float moveSpeed = 7f;
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

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than 1 player instance!");
        }
        Instance = this;
    }

    private void Start()
    {
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

        // If we hit something
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                Debug.Log(raycastHit.transform.name);
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
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Cannot move in this dir
            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move in any Direction
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
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