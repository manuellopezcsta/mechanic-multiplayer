using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCounter : BaseCounter
{
    // Codigo de los botones del elevador.
    [SerializeField] ElevatorController elevatorController;
    [SerializeField] ParticleSystem[] effects;
    void Start()
    {
        if (effects.Length != 0)
        {
            foreach (ParticleSystem efect in effects)
            {
                efect.Stop();
            }
        }
    }
    public override void Interact(Player player)
    {
        elevatorController.ChangeFloorElevator();
        Debug.Log("Piso actual: " + elevatorController.floorNumberElevator);
        ActiveEfects();
    }
    private void ActiveEfects()
    {
        if (effects.Length != 0)
        {
            foreach (ParticleSystem efect in effects)
            {
                efect.Play();
            }
        }
    }
}