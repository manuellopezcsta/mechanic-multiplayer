using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class OWPlayerPresenter : MonoBehaviour
{
    private OWPlayerModel playerModel;
    private PlayerInput playerInputs;
    private CharacterController characterController;
    private Vector2 inputVector;
    private Vector3 castDirection;
    [SerializeField] private LayerMask levelLayerMask;
    public Action OnPause { get; set; }
    public Action<Quaternion> OnMovingRotate { get; set; }
    public Action<bool> MoveAnimation { get; set; }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerModel = GetComponent<OWPlayerModel>();
        var playerConfig = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray()[0];
        playerInputs = playerConfig.PInput;
        playerInputs.defaultActionMap = "WorldSelect";
        Debug.Log(playerInputs.playerIndex);

        // Configurar los eventos del PlayerInput
        playerInputs.actions["Interact"].performed += Interact;
        playerInputs.actions["Move"].performed += Move_performed;
        playerInputs.actions["Move"].canceled += Move_canceled;
        playerInputs.actions["Pause"].performed += Pause;
    }

    public void OnDestroy()
    {
        Debug.Log("Se ejecuto el OnDestroy Del PlayerController Del WorldSelect ");
        Debug.Log(playerInputs != null);
        if (!playerInputs) return;
        playerInputs.actions["Interact"].performed -= Interact;
        playerInputs.actions["Move"].performed -= Move_performed;
        playerInputs.actions["Move"].canceled -= Move_canceled;
        playerInputs.actions["Pause"].performed -= Pause;
    }
    public void Interact(InputAction.CallbackContext context)
    {
        //Interaction capsule sizes
        float capsuleDistance = 6f;
        float capsuleRadius = 1.5f;
        float capsuleHeight = 3f;

        Vector3 capsuleStart = transform.position;
        Vector3 capsuleEnd = transform.position + Vector3.up * capsuleHeight;

        if (castDirection == Vector3.zero)
        {
            castDirection = playerModel.CalculateMove(inputVector); //Make facing and interaction direction the same
        }
        Debug.Log("cast direction: " + castDirection);

        if (Physics.CapsuleCast(capsuleStart, capsuleEnd, capsuleRadius, castDirection, out RaycastHit raycastHit, capsuleDistance, levelLayerMask))
        {//Raycast hiting a level select object
            if (raycastHit.transform.TryGetComponent(out WorldObject worldObject))
            {
                worldObject.Interact();
            }
        }
    }
    public void SetInputVector(Vector2 direction)
    {
        inputVector = direction.normalized;
    }
    private void Move_performed(InputAction.CallbackContext context)
    {

        MoveAnimation?.Invoke(true);
        SetInputVector(context.ReadValue<Vector2>());
        castDirection = playerModel.CalculateMove(inputVector); //Make facing and interaction direction the same

    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        MoveAnimation?.Invoke(false);
        // Evita que se ejecute si el botón aún está presionado
        SetInputVector(Vector2.zero);
    }

    public void Pause(InputAction.CallbackContext context)
    {
        OnPause?.Invoke();
    }
    public bool IsWalking()
    {
        //Check if the character is currently moving
        return inputVector != Vector2.zero;

    }

    void Update()
    {
        if (!IsWalking()) //If i'm not walking return the update early
        {
            return;
        }
        if (characterController != null)
        {
            characterController.Move(playerModel.CalculateMove(inputVector));
            OnMovingRotate?.Invoke(playerModel.CalculateRotation(inputVector));

        }
        else
        {
            Debug.LogError("CharacterController unasigned.");
        }
    }
}
