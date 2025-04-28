using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private ObjectsSO objectsSO;
    private int maxGeneratedObjects;
    [SerializeField] private List<Transform> generatedObjects;

    private void Start()
    {
        maxValueGeneratedObjects();
    }
    public override void Interact(Player player)
    {
        //ademas de preguntar si el player no tiene un objeto verificamos que los objetos generados sean menores que el maximo permitido
        if (!player.HasCarObject() && maxGeneratedObjects > generatedObjects.Count)
        {
            // Si el player no tiene nada en la mano, spawneamos 1
            Transform carObjectTransform = Instantiate(objectsSO.prefab);
            carObjectTransform.GetComponent<CarObject>().SetCarObjectParent(player);
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
            generatedObjects.Add(carObjectTransform);
        }
    }

    //Sacamos del levelProperties el valor maximo de cada objeto a crear y lo seteamos
    private void maxValueGeneratedObjects()
    {
        switch (objectsSO.name)
        {
            case "Aceite":
                maxGeneratedObjects = GameManager.Instance.GetLevelProperties().maxOilObjects;
                generatedObjects = GameManager.Instance.generatedOilObjects;
                Debug.Log("Se pueden instanciar " + maxGeneratedObjects + " " + objectsSO.name);
                break;
            case "Caja":
                maxGeneratedObjects = GameManager.Instance.GetLevelProperties().maxBoxObjects;
                generatedObjects = GameManager.Instance.generatedBoxObjects;
                Debug.Log("Se pueden instanciar " + maxGeneratedObjects + " " + objectsSO.name);
                break;
            case "Wheel":
                generatedObjects = GameManager.Instance.generatedWheelObjects;
                maxGeneratedObjects = GameManager.Instance.GetLevelProperties().maxWheelObjects;
                Debug.Log("Se pueden instanciar " + maxGeneratedObjects + " " + objectsSO.name);
                break;
            case "SparkPlug":
                maxGeneratedObjects = GameManager.Instance.GetLevelProperties().maxSparkPlugObjects;
                generatedObjects = GameManager.Instance.generatedSparkPlugObjects;
                Debug.Log("Se pueden instanciar " + maxGeneratedObjects + " " + objectsSO.name);
                break;
            case "Fusible":
                maxGeneratedObjects = GameManager.Instance.GetLevelProperties().maxFuseObjects;
                generatedObjects = GameManager.Instance.generatedFuseObjects;
                Debug.Log("Se pueden instanciar " + maxGeneratedObjects + " " + objectsSO.name);
                break;
        }
    }

}
