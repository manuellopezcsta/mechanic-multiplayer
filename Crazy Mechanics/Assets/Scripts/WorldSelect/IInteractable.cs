using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    public UnityEvent OnInteract{get; protected set;}
    public void Interact();
}
