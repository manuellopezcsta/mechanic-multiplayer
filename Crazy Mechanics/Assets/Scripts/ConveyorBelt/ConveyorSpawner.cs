using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSpawner : MonoBehaviour
{
    private IEnumerator spawnTimeOut;
    [SerializeField] private float spawnTimer = 10f;
    public bool spawnOK = true;

    void Start()
    {
        StartCoroutine(SpawnCounter());
    }
    private IEnumerator SpawnCounter()
    {
        yield return new WaitForSeconds(spawnTimer);
        GameObject counter = ConveyorBelt.instance.GetCounterToSpawn();
        if (counter != null)
        {
            counter.SetActive(true);
        }
        StartCoroutine(SpawnCounter());
    }
}

    
