using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class WorldObject : MonoBehaviour, IInteractable
{
    [SerializeField] string worldToGo = "Test Demo";
    [SerializeField] private UnityEvent _onInteract;
    UnityEvent IInteractable.OnInteract { 
        get => _onInteract;
        set => _onInteract  = value;   
    }

    public void Interact () => _onInteract.Invoke();

    public void LoadWorld () {
        Debug.Log("world object interaction");
        SceneManager.LoadScene(worldToGo);
    }

}