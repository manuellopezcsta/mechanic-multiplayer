using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, ICarObjectParent
{
    [SerializeField] private Transform counterTopPoint;

    private CarObject carObject;

    public virtual void Interact(Player player) {
        Debug.LogError("Se ejecuto BaseCounter Interact, falta implementar.!");
    }

    public Transform GetCarObjectFollowTransform() {
        return counterTopPoint;
    }

    public void SetCarObject(CarObject target) {
        this.carObject = target;
    }

    public CarObject GetCarObject() {
        return carObject;
    }

    public void ClearCarObject() {
        carObject = null;
    }

    public bool HasCarObject() {
        return carObject != null;
    }
}
