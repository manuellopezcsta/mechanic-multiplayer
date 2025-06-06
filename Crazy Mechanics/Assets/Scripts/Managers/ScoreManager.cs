using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshPro scoreDisplay;

    [SerializeField] private int totalScore = 0;
    private float currentScore = 0; // Usado para animar la subida de puntaje.
    private float scoreIncreaseRate = 0.05f;

    private int carsDelivered = 0;
    [SerializeField] private TextMeshPro textCarsDelivered;
    [SerializeField] private int multiplierPlayerValue = 0; //Multiplicador por cantidad de players existentes, Default : 0 
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
        //UpdateScoreDisplay();
    }

    /* // Modo de update Rapido
    public void UpdateScoreDisplay() {
        scoreDisplay.text = "$" + totalScore.ToString();
    }*/

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
        }
    }

    public void CarsDelivered()
    {
        carsDelivered++;
        textCarsDelivered.text = carsDelivered.ToString();
    }

    public int GetScore()
    {
        return totalScore;
    }

    private void GetMultiplierValue()
    {
        switch (GameManager.inputHandlersList.Count)
        {
            case 1:
                multiplierPlayerValue = 4;
                break;
            case 2:
                multiplierPlayerValue = 3;
                break;
            case 3:
                multiplierPlayerValue = 2;
                break;
            case 4:
                multiplierPlayerValue = 1;
                break;
        }
    }
}
