using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshPro scoreDisplay;

    [SerializeField] private float totalScore = 0;
    private float currentScore = 0; // Usado para animar la subida de puntaje.
    private float scoreIncreaseRate = 0.05f;

    [SerializeField] private float multiplierPlayerValue = 0; //Multiplicador por cantidad de players existentes, Default : 0 
    private float[] valuesMulplier = {2f,1.5f,1f,0.5f};
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        GetMultiplierValue(); //Si no se corre en el Start el multiplierPlayerValue es 0;
    }
    public void AddPoints(int ammount)
    {
        totalScore += ammount * multiplierPlayerValue;
    }

    // Modo de update Rapido
    public void UpdateScoreDisplay() {
        scoreDisplay.text = "$" + totalScore.ToString();
    }

    // Modo de updatear el score mas bonito visualmente..
    private void Update()
    {
        if (currentScore < totalScore)
        {
            currentScore += scoreIncreaseRate;
            if (currentScore > totalScore)
            {
                currentScore = totalScore;
            }
            scoreDisplay.text = "$" + ((int)currentScore).ToString();
        }else if (totalScore < currentScore)
        {
            currentScore -= scoreIncreaseRate;
            if (totalScore > currentScore)
            {
                currentScore = totalScore;
            }
            scoreDisplay.text = "$" + ((int)currentScore).ToString();
        }
        
    }


    public float GetScore()
    {
        return totalScore;
    }

    private void GetMultiplierValue()
    {
        switch (GameManager.inputHandlersList.Count)
        {
            case 1:
                multiplierPlayerValue = valuesMulplier[0];
                break;
            case 2:
                multiplierPlayerValue = valuesMulplier[1];
                break;
            case 3:
                multiplierPlayerValue = valuesMulplier[2];
                break;
            case 4:
                multiplierPlayerValue = valuesMulplier[3];
                break;
        }
    }
}
