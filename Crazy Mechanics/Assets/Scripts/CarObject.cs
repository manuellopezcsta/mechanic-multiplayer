using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarObject : MonoBehaviour
{
    [SerializeField] private ObjectsSO objectsSO;

    private ICarObjectParent carObjectParent;

    public ObjectsSO GetObjectSO()  {
        return objectsSO;
    }

    public void SetCarObjectParent(ICarObjectParent targetParent) {
        if(this.carObjectParent != null) {
            this.carObjectParent.ClearCarObject();
        }
        
        this.carObjectParent = targetParent;

        if (targetParent.HasCarObject()) {
            Debug.LogError("TargetParent Ya tiene un objeto.");
        }

        targetParent.SetCarObject(this);

        transform.parent = carObjectParent.GetCarObjectFollowTransform();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    } 

    public ICarObjectParent GetCarObjectParent() {
        return carObjectParent;
    }
}
