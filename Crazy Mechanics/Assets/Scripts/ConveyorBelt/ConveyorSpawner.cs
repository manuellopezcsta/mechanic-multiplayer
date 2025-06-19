using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSpawner : MonoBehaviour
{
    private IEnumerator spawnTimeOut;
    [SerializeField] private ConveyorBelt conveyorBelt;
    [SerializeField] private float spawnTimer = 10f;
    public bool spawnOK = true;

    void Start()
    {
        StartCoroutine(SpawnCounter());
    }
    private IEnumerator SpawnCounter()
    {
        yield return new WaitForSeconds(spawnTimer);
        GameObject counter = conveyorBelt.GetCounterToSpawn();
        if (counter != null)
        {
            counter.SetActive(true);
        }
        StartCoroutine(SpawnCounter());
    }
}

    
