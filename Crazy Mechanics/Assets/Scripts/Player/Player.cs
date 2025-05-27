using System;
using UnityEngine;


public class Player : MonoBehaviour, ICarObjectParent
{
    public static bool invertControls = false;
    public bool isSliding;
    public Vector3 slideDir;

    // Eventos
    public static event EventHandler OnPickedSomething;
    public event EventHandler OnStun;

    [SerializeField] private Transform carObjectHoldPoint;

    public bool isWalking = false;
    public CarObject carObject;
    public float speed = 7f;
    public Vector3 moveDir;
    public BaseCounter selectedCounter;

    // Para Hacer mas chiquito el player
    private PlayerMovement playerMovement;
    private PlayerInteract playerInteract;
    private void Awake()
    {
        GameManager.RegisterPlayer(this);
    }

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInteract = GetComponent<PlayerInteract>();
    }

    private void Update()
    {
        playerMovement.HandleMovement();
        playerInteract.HandleInteractions();
    }

    public void TriggerStunEvent()
    {
        OnStun?.Invoke(this, EventArgs.Empty);
    }

    public Transform GetCarObjectFollowTransform()
    {
        return carObjectHoldPoint;
    }

    public void SetCarObject(CarObject target)
    {
        this.carObject = target;

        if (this.carObject != null)
        {
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