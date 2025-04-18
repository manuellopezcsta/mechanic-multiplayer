using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent _onInteract;
    [SerializeField] private Transform transformSelf;
    private Transform grabPosition;
    
    UnityEvent IInteractable.OnInteract { 
        get => _onInteract;
        set => _onInteract  = value;   
    }

    public void Interact () => _onInteract.Invoke();  
    

    public void Grab (Transform newHoldPoint) {
        this.grabPosition = newHoldPoint;
        transformSelf.position = newHoldPoint.position;
        transformSelf.SetParent(grabPosition);
        

    }
}
