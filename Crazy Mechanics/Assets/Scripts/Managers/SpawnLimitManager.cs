using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLimitManager : MonoBehaviour
{
    public static SpawnLimitManager Instance { get; private set; }

    const string OIL_OBJECTSO_NAME = "Aceite";
    const string BOX_OBJECTSO_NAME = "Caja";
    const string WHEEL_OBJECTSO_NAME = "Wheel";
    const string SPARKPLUG_OBJECTSO_NAME = "SparkPlug";
    const string FUSE_OBJECTSO_NAME = "Fusible";
    const string PISTON_OBJECTSO_NAME = "Piston";
    
    //Contadores de los objetos generados segun el tipo
    private int oilCounter;
    private int wheelCounter;
    private int sparkPlugCounter;
    private int pistonCounter;
    private int fuseCounter;
    private int boxCounter;

    LevelProperties levelProperties;


    private void Awake()
    {
        Instance = this;
        levelProperties = GameManager.Instance.GetLevelProperties();
    }

    public int GetItemSpawnLimit(string targetName) {
        switch(targetName) {
            case OIL_OBJECTSO_NAME:
                return levelProperties.maxOilObjects;
            case BOX_OBJECTSO_NAME:
                return levelProperties.maxBoxObjects;
            case WHEEL_OBJECTSO_NAME:
                return levelProperties.maxWheelObjects;
            case SPARKPLUG_OBJECTSO_NAME:
                return levelProperties.maxSparkPlugObjects;
            case PISTON_OBJECTSO_NAME:
                return levelProperties.maxPistonObjects;
            case FUSE_OBJECTSO_NAME:
                return levelProperties.maxFuseObjects;
            default:
                return 0;
        }
    }

    public int GetSpawnedItemsCount(string targetName) {
        switch(targetName) {
            case OIL_OBJECTSO_NAME:
                return oilCounter;
            case BOX_OBJECTSO_NAME:
                return boxCounter;
            case WHEEL_OBJECTSO_NAME:
                return wheelCounter;
            case SPARKPLUG_OBJECTSO_NAME:
                return sparkPlugCounter;
            case PISTON_OBJECTSO_NAME:
                return pistonCounter;
            case FUSE_OBJECTSO_NAME:
                return fuseCounter;
            default:
                return 0;
        }
    }

    // Esto suma y resta a los array, cuando se elimina un item se llama con un valor de -1.
    public void ModifySpawnedCounter(string targetName, int ammount) {
        switch(targetName) {
            case OIL_OBJECTSO_NAME:
                oilCounter += ammount;
                break;
            case BOX_OBJECTSO_NAME:
                boxCounter += ammount;
                break;
            case WHEEL_OBJECTSO_NAME:
                wheelCounter += ammount;
                break;
            case SPARKPLUG_OBJECTSO_NAME:
                sparkPlugCounter += ammount;
                break;
            case FUSE_OBJECTSO_NAME:
                fuseCounter += ammount;
                break;
            case PISTON_OBJECTSO_NAME:
                pistonCounter += ammount;
                break;
            default:
                //Debug.LogWarning("ERROR ! NO EXISTE ESE ITEM LIST!");
                break;
        }
    }
}
