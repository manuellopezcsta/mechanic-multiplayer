using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] private int totalScore = 0;
    public void SubmitScoreTotal(int subTotal){
        totalScore += subTotal;
    }
}
