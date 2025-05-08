using System.Collections;
using System.Collections.Generic;
using Deform;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CarBender : MonoBehaviour
{
    private Deformable deformable;
    private GameObject deformer;

    void Awake()
    {

        deformable = transform.GetChild(0).AddComponent<Deformable>(); //AddComponent<Deformable>();
        //Debug.Log(deformable!=null);
        deformable.UpdateMode = UpdateMode.Auto; //Cambiar medio de actualización a automatico para que refleje los cambios de valor
        if(deformer){//Si ya posee un scrip deformer lo remueve y destruye
            deformable.RemoveDeformer(deformer.GetComponent<Deformer>());
            Destroy(deformer);
        }

    }

    public void Bend(){
        deformer = new GameObject("Deformer", typeof(PerlinNoiseDeformer)); //agrego deformer basado en ruido de Perlin al objeto
        deformer.GetComponent<PerlinNoiseDeformer>().MagnitudeScalar = -0.2f; //Modifico valor de magnitud del ruido de perlin para generar efecto de abolladura
        deformable.AddDeformer(deformer.GetComponent<Deformer>());
        deformer.transform.parent = transform;
    }

    public void UnBend(float value){
        float BendValue = deformer.GetComponent<PerlinNoiseDeformer>().MagnitudeScalar;
        BendValue = Mathf.Min(BendValue+value,0);
        deformer.GetComponent<PerlinNoiseDeformer>().MagnitudeScalar=BendValue;
    }
}
