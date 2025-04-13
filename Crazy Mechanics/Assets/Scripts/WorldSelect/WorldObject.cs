using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldObject : BaseCounter
{
     [SerializeField] public string WorldToGo = "Test Demo"; //Por default se asigna "Test Demo"

    public override void Interact(Player player)
    {
        SceneManager.LoadScene (WorldToGo);
    }
}
