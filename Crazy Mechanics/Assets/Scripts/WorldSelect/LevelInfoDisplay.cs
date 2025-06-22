using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoDisplay : MonoBehaviour
{
    [SerializeField] private string levelNumber;
    [SerializeField] private TextMeshProUGUI scoreField;
    [SerializeField] private GameObject icon;
    int score = 0;

    void Awake()
    {   
        
        if(PlayerPrefs.HasKey(levelNumber)){
            score = PlayerPrefs.GetInt(levelNumber + "Score");
            scoreField.text = score.ToString();
            icon.SetActive(true);
        }
        else{
            scoreField.text = "INCOMPLETE";
        }
    }
}
