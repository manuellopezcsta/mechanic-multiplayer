using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    private float gamePlayingTimer;
    private bool running;
    [SerializeField] private float gamePlayingTimerMax = 500f; // tiempo del nivel
    [SerializeField] private TextMeshProUGUI timerText;


    public void StartTimer()
    {
        running = true;
        gamePlayingTimer = gamePlayingTimerMax;
    }

    void Update()
    {
        gamePlayingTimer -= Time.deltaTime;
        //print(" " + gamePlayingTimer);
        if (gamePlayingTimer < 0f && running)
        {
            // Codigo cuando se pierde
            Debug.Log("Se perdio");
            running = false;
        }

        // Updateamos la visual.
        TimeSpan UptimeSpan = TimeSpan.FromSeconds(gamePlayingTimer);//Utilizo TimeSpan para formatear el tiempo
        timerText.text = UptimeSpan.ToString(format:@"mm\:ss\:ff");
    }
}
