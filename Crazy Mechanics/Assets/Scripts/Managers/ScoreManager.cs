using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI scoreDisplay;

    [SerializeField] private int totalScore = 0;
    private float currentScore = 0; // Usado para animar la subida de puntaje.
    private float scoreIncreaseRate = 0.05f;
    private void Awake()
    {
        Instance = this;
    }
    public void AddPoints(int ammount)
    {
        totalScore += ammount;
        //UpdateScoreDisplay();
    }
    
    /* // Modo de update Rapido
    public void UpdateScoreDisplay() {
        scoreDisplay.text = "$" + totalScore.ToString();
    }*/

    // Modo de updatear el score mas bonito visualmente..
    private void Update()
    {
        if(currentScore < totalScore)
        {
            currentScore += scoreIncreaseRate;
            if(currentScore > totalScore) {
                currentScore = totalScore;
            }
            scoreDisplay.text = "$" + ((int)currentScore).ToString();
        }
    }

    public int GetScore(){
        return totalScore;
    }
}
