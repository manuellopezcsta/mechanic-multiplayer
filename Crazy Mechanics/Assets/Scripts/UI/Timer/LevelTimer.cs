using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }
    private float gamePlayingTimer;
    private bool running;
    [SerializeField] private TextMeshPro timerText;
    [SerializeField] private ScorePanelUI scorePanelUI;

    void Awake()
    {
        Instance = this;
    }

    public void StartTimer(float gamePlayingTimerMax)
    {
        gamePlayingTimer = gamePlayingTimerMax;
        running = true;
    }

    void Update()
    {
        gamePlayingTimer -= Time.deltaTime;
        //print(" " + gamePlayingTimer);
        if (gamePlayingTimer < 0f && running)
        {
            ExitLevel();
        }

        // Updateamos la visual.
        TimeSpan UptimeSpan = TimeSpan.FromSeconds(gamePlayingTimer);//Utilizo TimeSpan para formatear el tiempo
        timerText.text = UptimeSpan.ToString(format: @"hh\:mm\:ss");
        //timerText.text = UptimeSpan.ToString(format:@"mm\:ss\:ff");
    }
    public void ExitLevel()
    {
        // Codigo cuando se termina el tiempo
        Debug.Log("Se termino el nivel");
        scorePanelUI.Show();
        running = false;
        SoundManager.Instance.PlayEndOfLevelSound();
    }
}
