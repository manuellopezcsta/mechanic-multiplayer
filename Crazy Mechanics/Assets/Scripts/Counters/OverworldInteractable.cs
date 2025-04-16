using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldInteractable : MonoBehaviour //, ICarObjectParent
{
    private CarObject representingObject;
    [SerializeField] private Transform objectTransform;

    public virtual void Interact (PlayerWorldSelect player){

    }

    public Transform GetRepreseintingObjectTransform(){
        return objectTransform;//return transform of the interactable object
    }

    public void SetRepreseintingObject(CarObject thing){ //set a "thing" as the object to be represente by the interactable
        this.representingObject = thing;
    }

    public CarObject GetCarObject() { //returns what object the interactable represents
        return representingObject;
    }

    public void ClearCarObject () {
        representingObject = null;        
    }

    public bool HasCarObject () {
        return representingObject != null;
    }
}
