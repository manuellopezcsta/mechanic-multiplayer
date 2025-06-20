
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] Transform[] goals;
    [SerializeField] Transform startPoint;
    [SerializeField] GameObject conveyorCounterPrefab;
    [SerializeField] int counterQunatity;
    [SerializeField] private float speed = 5f;
    public int itemSpawnLimit;
    [SerializeField] private ObjectsSO[] objectsSO;
    private List<GameObject> counterPool = new List<GameObject>();
    private int lastIndex = 0;
    [SerializeField] private float randomBase;
    void Awake()
    {

    }

    void Start()
    {
        for (int i = 0; i < counterQunatity; i++)
        {
            GameObject conveyorCounter = Instantiate(conveyorCounterPrefab, startPoint.position, Quaternion.identity);
            conveyorCounter.GetComponent<ConveyorCounter>().setUP(startPoint, goals, speed);
            counterPool.Add(conveyorCounter);
            conveyorCounter.SetActive(false);
            
        }
    }

    public GameObject GetCounterToSpawn()
    {
    if (counterPool != null) {
           
        for (int i = lastIndex; counterPool.Count > i; i++)
            {
                int value = Random.Range(0, objectsSO.Length);
                itemSpawnLimit = SpawnLimitManager.Instance.GetItemSpawnLimit(objectsSO[value].name);
                GameObject counter = counterPool[i];
                ConveyorCounter counterCC = counter.GetComponent<ConveyorCounter>();
                if (!counter.activeInHierarchy)
                {
                    //Debug.Log("Found a counter index" + i);
                    lastIndex = i+1;
                    if (lastIndex == counterPool.Count)
                    {
                        lastIndex=0;
                    }
                    if (objectsSO == null)
                    {
                        return counter;
                    }
                    else if (!counterCC.HasCarObject() && itemSpawnLimit > SpawnLimitManager.Instance.GetSpawnedItemsCount(objectsSO[value].name) && Random.Range(0f, randomBase) < 1f)
                    {
                        //Si el counter no tiene nada, y todavia puedo spawnear objetos en el nivel, posiblemente spawnea un item en el counter
                        Transform carObjectTransform = Instantiate(objectsSO[value].prefab);
                        carObjectTransform.GetComponent<CarObject>().SetCarObjectParent(counterCC);
                        SpawnLimitManager.Instance.ModifySpawnedCounter(objectsSO[value].name, 1);
                    }
                    return counter;
                }
            }
    }
        return null;
    }
}
