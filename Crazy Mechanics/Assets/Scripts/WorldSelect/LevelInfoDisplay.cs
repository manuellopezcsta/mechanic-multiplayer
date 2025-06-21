using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelInfoDisplay : MonoBehaviour
{
    [SerializeField] private string levelNumber;
    [SerializeField] private TextMeshProUGUI scoreField;
    int score = 0;

    void Awake()
    {   
        
        if(PlayerPrefs.HasKey(levelNumber)){
            score = PlayerPrefs.GetInt(levelNumber + "Score");
            scoreField.text = score.ToString();
        }
        else{
            scoreField.text = "INCOMPLETE";
        }
    }
}
