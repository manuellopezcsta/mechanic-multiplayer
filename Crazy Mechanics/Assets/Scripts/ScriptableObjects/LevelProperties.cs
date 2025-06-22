using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelProperties : ScriptableObject
{   
    [Header("Number/Time")]
    public string levelNumber;
    public int levelTime;
    [Header("Stars")]
    public int firstStarScore;
    public int secondStarScore;
    public int thirdStarScore;
    [Header("Tasks")]
    public int minTaskNumber;
    public int maxTaskNumber;
    [Header("Allowed Tasks")]
    public List<GameManager.CarTasks> listTasks = new List<GameManager.CarTasks>();
    [Header("Spawn Limits")]
    public int maxOilObjects;
    public int maxBoxObjects;
    public int maxFuseObjects;
    public int maxSparkPlugObjects;
    public int maxPistonObjects;
    public int maxWheelObjects;
    [Header("Disaster Times")]
    public List<DisasterManager.DisasterType> disasterTypes = new List<DisasterManager.DisasterType>();
    public int disasterMinTimer;
    public int disasterMaxTimer;
    [Header("Level Tutorial Data")]
    public bool hasTutorial;
    public Sprite[] tutorialImage;
    
}
