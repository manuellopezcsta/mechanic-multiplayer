using System.Collections;
using System.Collections.Generic;
using Deform;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CarBender : MonoBehaviour
{
    private Deformable bendableObject; // El objeto a deformar
    private GameObject deformer; // El deformador

    public float deformedNoiseValue = -0.2f;

    void Awake()
    {
        // Agregamos el objeto deformable a el Body del auto que es el mesh para poder cambiarlo
        bendableObject = transform.GetChild(0).AddComponent<Deformable>(); //AddComponent<Deformable>();
        //Debug.Log(deformable!=null);
        //Cambiar medio de actualización a automatico para que refleje los cambios de valor
        bendableObject.UpdateMode = UpdateMode.Auto;
        //Si ya posee un scrip deformer lo remueve y destruye
        if (deformer)
        {
            bendableObject.RemoveDeformer(deformer.GetComponent<Deformer>());
            Destroy(deformer);
        }
    }

    // Funcion que se llama cuando se crea la task.
    public void Bend()
    {
        //Agrego deformer basado en ruido de Perlin al objeto
        deformer = new GameObject("Deformer", typeof(PerlinNoiseDeformer)); 
        //Modifico valor de magnitud del ruido de perlin para generar efecto de abolladura
        deformer.GetComponent<PerlinNoiseDeformer>().MagnitudeScalar = deformedNoiseValue; 
        bendableObject.AddDeformer(deformer.GetComponent<Deformer>());
        deformer.transform.parent = transform;
    }

    // Funcion que se llama desde el interact para arreglar el objeto de a poquito.
    public void Unbend(float value)
    {
        float BendValue = deformer.GetComponent<PerlinNoiseDeformer>().MagnitudeScalar;
        BendValue = Mathf.Min(BendValue + value, 0);
        deformer.GetComponent<PerlinNoiseDeformer>().MagnitudeScalar = BendValue;
    }
}
