using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMannager : MonoBehaviour
{
    private int lastLevelLoaded;
    [SerializeField] Transform[] levelSpawns;
    [SerializeField] Transform playerPosition;

    void Start()
    {
        playerPosition.transform.position = levelSpawns[lastLevelLoaded].transform.position;
    }

    public void changeLasteLevelLoade(int level)
    {
        lastLevelLoaded = level;
    }

    public int getLastLevelLoaded()
    {
        return lastLevelLoaded;
    }
}
