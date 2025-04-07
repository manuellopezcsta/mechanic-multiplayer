using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonStart : BaseCounter
{
    [SerializeField] private GameObject carPreFab;
    [SerializeField] private Transform positionInstantiate;
    private CarController carController;
    public override void Interact(Player player)
    {
        GameObject car = Instantiate(carPreFab,positionInstantiate);
        Debug.Log("Boop");
        carController = car.GetComponent<CarController>();
        Debug.Log(car.name);
        carController.canMove = true;
    }
}
