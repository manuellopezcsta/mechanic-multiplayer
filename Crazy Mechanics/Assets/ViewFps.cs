using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViewFps : MonoBehaviour
{
    public TextMeshProUGUI visualText;

    private float deltaTime = 0;
    private bool activo;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5)){
            activo = !activo;
        }

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1/deltaTime;

        if(activo){
            visualText.text = Mathf.Ceil(fps).ToString() + " FPS";
        }else{
            visualText.text = "";
        }
    }
}
