using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMannager : MonoBehaviour
{
    public static OverworldMannager Instance { get; private set; }
    private int lastLevelLoaded = 0;
    const string KEY = "LastLevelLoaded";
    [SerializeField] Transform[] levelSpawns;
    [SerializeField] Transform playerPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (PlayerPrefs.HasKey(KEY))//Si ya esta cargada un ultimo nivel accedido de la ultima sesión lo carga
        {
            lastLevelLoaded = PlayerPrefs.GetInt(KEY);
        }
        playerPosition.transform.position = levelSpawns[lastLevelLoaded].transform.position;
    }

    public void ChangeLastLevelLoaded(int level)
    {
        PlayerPrefs.SetInt(KEY, level);
        //Debug.Log("Seting last level to: " + PlayerPrefs.GetInt(KEY) + "spawn position: " + levelSpawns[lastLevelLoaded].transform);
    }

    public int GetLastLevelLoaded()
    {
        return lastLevelLoaded;
    }
}
