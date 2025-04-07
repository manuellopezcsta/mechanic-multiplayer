using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICarObjectParent
{
    public Transform GetCarObjectFollowTransform();

    public void SetCarObject(CarObject target);

    public CarObject GetCarObject();

    public void ClearCarObject();

    public bool HasCarObject();
}
