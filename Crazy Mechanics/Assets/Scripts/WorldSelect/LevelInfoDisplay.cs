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
        
        //scoreField = GetComponent<TextMeshProUGUI>();
        scoreField.text = "before if";
        if(PlayerPrefs.HasKey(levelNumber)){
            score = PlayerPrefs.GetInt(levelNumber);
            scoreField.text = levelNumber.ToString();
        }
        else{
            scoreField.text = "didn't enter IF";
        }
    }
}
