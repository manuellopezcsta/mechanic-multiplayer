using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSelectManager : MonoBehaviour
{
    [SerializeField] public string WorldToGo = "Test Demo"; //Por default se asigna "Test Demo"

    public void ActivateWorld () {
        SceneManager.LoadScene (WorldToGo);
    }
}
