using System;
using UnityEngine;

public class WorldSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject StagePanel;
    [SerializeField] private CanvasGroup Canvas;
    private bool FadeIn = false;
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")){
            Debug.Log("fading in/n");
            FadeIn = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")){
            Debug.Log("fading out/n");
            FadeIn = false;
        }
    }

    void Update()
    {
        if(FadeIn){
            if(Canvas.alpha < 1){
                Canvas.alpha+=Time.deltaTime;
            }
            
        }
        
        else{
            if(Canvas.alpha>0){
                Canvas.alpha-=Time.deltaTime;
            }  
        }
    }
}
