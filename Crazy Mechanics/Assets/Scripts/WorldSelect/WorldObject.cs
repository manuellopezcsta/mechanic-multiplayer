using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class WorldObject : MonoBehaviour, IInteractable
{
    [SerializeField] string worldToGo = "Test Demo";
    [SerializeField] private UnityEvent _onInteract;
    [SerializeField] Animator transitionAnimation;
    UnityEvent IInteractable.OnInteract { 
        get => _onInteract;
        set => _onInteract  = value;   
    }

    IEnumerator LoadLevel(){
        transitionAnimation.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(worldToGo);
        transitionAnimation.SetTrigger("Start");
    }
    public void Interact () => _onInteract.Invoke();

    public void LoadWorld () {
       
        Debug.Log("world object interaction");
        StartCoroutine(LoadLevel());
    }

}