using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class WorldObject : MonoBehaviour, IInteractable
{
    // Cambiar por un Enum de tipo Scene y usar el Loader.Load(worldTogo)
    [SerializeField] Loader.Scene worldToGo = Loader.Scene.Level0;
    [SerializeField] private UnityEvent _onInteract;
    [SerializeField] Animator transitionAnimation;
    [SerializeField] int levelNumber;
    //[SerializeField] private OverworldMannager owMannager;
    UnityEvent IInteractable.OnInteract { 
        get => _onInteract;
        set => _onInteract  = value;   
    }

    IEnumerator LoadLevel(){
        transitionAnimation.SetTrigger("End");
        yield return new WaitForSeconds(1);
        Loader.Load(worldToGo);
        transitionAnimation.SetTrigger("Start");
    }
    public void Interact () => _onInteract.Invoke();

    public void LoadWorld () {
        OverworldMannager.Instance.ChangeLastLevelLoaded(levelNumber);
        Debug.Log("world object interaction");
        StartCoroutine(LoadLevel());
    }

}