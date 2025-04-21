using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelProperties : ScriptableObject
{
    public int levelTime;
    public int firstStarScore;
    public int secondStarScore;
    public int thirdStarScore;
    public string levelNumber;
    
    public int minTaskNumber;
    public int maxTaskNumber;

    public List<GameManager.CarTasks> listTasks = new List<GameManager.CarTasks>();
}
