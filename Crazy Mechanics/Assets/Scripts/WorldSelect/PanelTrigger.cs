using System;
using UnityEngine;

public class WorldSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject StagePanel;
    [SerializeField] private CanvasGroup Canvas;
    private bool FadeIn = false;
    private bool FadeOut = false;
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")){
            //StagePanel.SetActive(true);
            Debug.Log("fading in/n");
            FadeIn=true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")){
            //StagePanel.SetActive(false);
            Debug.Log("fading out/n");
            FadeOut=true;
        }
    }

    void Update()
    {
        if(FadeIn){
            if(Canvas.alpha < 1){
                Canvas.alpha+=Time.deltaTime;
            }
            else{
                FadeIn=false;
            }
        }
        if(FadeOut && !FadeIn){ //Added "!FadeIn" to correct bug where one leave the area before the canvas completes the fade in animation causing both fade in and fed out to trigger simultaneusly
            if(Canvas.alpha>0){
                Canvas.alpha-=Time.deltaTime;
            }  
            else{
                FadeOut=false;
            }
        }
    }
}
