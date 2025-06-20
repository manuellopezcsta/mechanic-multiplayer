using UnityEngine;
using System;

public class PlayerInteract : MonoBehaviour
{
    private Player player;
    private Vector3 lastInteractDir;
    const string MOTOR_TOOL_TAG = "MotorTool";
    const string WALL_TAG = "Pared";
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform invisibleHolder;

    // Para tirar
    [Header("Throw")]
    [SerializeField] float throwMagnitude;
    [SerializeField] float fowardMagnitude;
    [SerializeField] float upMagnitude;

    public static event EventHandler OnDroppedSomething;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    public void HandleInteractions()
    {
        //Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        if (player.moveDir != Vector3.zero)
        {
            lastInteractDir = player.moveDir;
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
            if (raycastHit.transform.CompareTag(WALL_TAG))
            {
                SetSelectedCounter(null);
            }

            else if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Debug.Log(raycastHit.transform.name);
                // Has clear Counter
                if (baseCounter != player.selectedCounter)
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

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        player.selectedCounter = selectedCounter;
    }

    private void DebugInteractionCapsule(bool active, Vector3 capsuleStart, Vector3 capsuleEnd, float interactDistance, float capsuleRadius)
    {
        Vector3 offset = lastInteractDir.normalized * interactDistance;

        if (active)
        {
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

    /*public RaycastHit[] GetAllObjectInRange()//Obtiene todos los objetos en rango de interact
    {
        float interactDistance = 2f;
        float capsuleRadius = 0.5f; // Ajusta el radio seg�n sea necesario
        float capsuleHeight = 1.5f; // Ajusta la altura seg�n sea necesario

        Vector3 capsuleStart = transform.position;
        Vector3 capsuleEnd = transform.position + Vector3.up * capsuleHeight;

        return Physics.CapsuleCastAll(capsuleStart, capsuleEnd, capsuleRadius, lastInteractDir, interactDistance, countersLayerMask);
    }*/
    public RaycastHit? GetFirstInteractableObject()
    {
        float interactDistance = 2f;
        float capsuleRadius = 0.5f;
        float capsuleHeight = 1.5f;

        Vector3 capsuleStart = transform.position;
        Vector3 capsuleEnd = transform.position + Vector3.up * capsuleHeight;

        RaycastHit[] hits = Physics.CapsuleCastAll(capsuleStart, capsuleEnd, capsuleRadius, lastInteractDir, interactDistance, countersLayerMask);

        if (hits.Length == 0) return null;

        // Ordenar por distancia
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.CompareTag(WALL_TAG))
            {
                return null;
            }

            return hit;
        }

        return null;
    }

    public void HandleThrowing()
    {
        // Me desligo del objeto
        // Creamos un nuevo padre para el obj
        Transform holder = Instantiate(invisibleHolder);
        InvisibleHolder holderCounter = holder.GetComponent<InvisibleHolder>();
        holderCounter.thrownBy = player;
        //Activa el estado flying despues de 0.1 segundos
        StartCoroutine(holderCounter.SkipLaunchWindow());
        // Arreglamos la pos y rotacion
        holder.position = player.GetCarObjectFollowTransform().position;
        holder.rotation = transform.rotation;

        player.carObject.SetCarObjectParent(holderCounter);
        // Arreglo el tama;o del collider
        holderCounter.FixColliderSize();
        // Arreglo su visual
        holder.GetComponentInChildren<SelectedVisualInvisible>().SetUpSelected();
        // Lo tiro
        Vector3 forceDirection = ((transform.forward * fowardMagnitude) + Vector3.up * upMagnitude) * throwMagnitude;
        holderCounter.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
        // Reproducimos el sonidito
        OnDroppedSomething?.Invoke(this, EventArgs.Empty);
    }

    // Para empujar la pluma 
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        float forceMagnitude = 500f;

        // Logica para empujar la pluma
        if (rb != null && hit.gameObject.CompareTag(MOTOR_TOOL_TAG))
        {
            Vector3 forceDirection = hit.gameObject.transform.position - transform.position;

            forceDirection.y = 0;
            forceDirection.Normalize();

            //Debug.Log("force value  " + forceDirection * forceMagnitude * Time.deltaTime);
            rb.AddForceAtPosition(forceDirection * forceMagnitude * Time.deltaTime, transform.position, ForceMode.Impulse);
            return;
        }

        //Debug.Log(hit.gameObject.name);
        /*if ( rb != null && rb.velocity.magnitude > stunTreshHold && hit.gameObject.TryGetComponent(out InvisibleHolder item) )
        {
            if (item.thrownBy != this)
            {
                StartCoroutine(GetStunned());
            }
        }*/
    }
}
