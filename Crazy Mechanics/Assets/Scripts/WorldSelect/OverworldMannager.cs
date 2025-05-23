using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMannager : MonoBehaviour
{
    public static OverworldMannager instance { get; private set; }
    private int lastLevelLoaded;
    const string KEY = "LastLevelLoaded";
    [SerializeField] Transform[] levelSpawns;
    [SerializeField] Transform playerPosition;

    void Start()
    {
        playerPosition.transform.position = levelSpawns[lastLevelLoaded].transform.position;
        if (PlayerPrefs.HasKey(KEY))//Si ya esta cargada un ultimo nivel accedido de la ultima sesión lo carga
        {
            lastLevelLoaded = PlayerPrefs.GetInt(KEY);
        }
    }

    public void changeLasteLevelLoade(int level)
    {
        PlayerPrefs.SetInt(KEY, level);
    }

    public int getLastLevelLoaded()
    {
        return lastLevelLoaded;
    }
}
