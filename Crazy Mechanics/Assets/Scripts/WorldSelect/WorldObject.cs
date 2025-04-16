using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldObject : OverworldInteractable
{
    [SerializeField] public string WorldToGo = "Test Demo"; //Por default se asigna "Test Demo"
    public override void Interact(PlayerWorldSelect player)
    {
        SceneManager.LoadScene (WorldToGo);
    }


}
