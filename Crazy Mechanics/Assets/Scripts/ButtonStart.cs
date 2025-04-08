using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonStart : BaseCounter
{
    public override void Interact(Player player)
    {
        GameManager.Instance.GenerateCar(6);
    }
}
