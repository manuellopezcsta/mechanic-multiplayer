using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonStart : BaseCounter
{
    [SerializeField] private GameObject carPreFab;
    [SerializeField] private Transform positionInstantiate;
    [SerializeField] private CarController carController;
    public override void Interact(Player player)
    {
        GameObject car = Instantiate(carPreFab,positionInstantiate);
        carController = car.GetComponent<CarController>();
        carController.canMove = true;
    }
}
