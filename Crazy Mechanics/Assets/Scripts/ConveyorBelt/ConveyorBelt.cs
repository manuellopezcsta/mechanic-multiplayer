
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public static ConveyorBelt instance;
    [SerializeField] Transform goal;
    [SerializeField] Transform startPoint;
    [SerializeField] GameObject conveyorCounterPrefab;
    [SerializeField] int counterQunatity;
    [SerializeField] private float speed = 5f;
    private List<GameObject> counterPool = new List<GameObject>();
    private int lastIndex = 0;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        for (int i = 0; i < counterQunatity; i++)
        {
            GameObject conveyorCounter = Instantiate(conveyorCounterPrefab, startPoint.position, Quaternion.identity);
            conveyorCounter.GetComponent<ConveyorCounter>().setUP(startPoint, goal, speed);
            counterPool.Add(conveyorCounter);
            conveyorCounter.SetActive(false);
            
        }
    }

    public GameObject GetCounterToSpawn()
    {
    if (counterPool != null) {
           
        for (int i = lastIndex; counterPool.Count > i; i++)
            {
                GameObject counter = counterPool[i]; ;
                if (!counter.activeInHierarchy)
                {
                    Debug.Log("Found a counter index" + i);
                    lastIndex = i+1;
                    if (lastIndex == counterPool.Count)
                    {
                        lastIndex=0;
                    }
                    return counter;
                }
            }
    }
        return null;
    }
}
