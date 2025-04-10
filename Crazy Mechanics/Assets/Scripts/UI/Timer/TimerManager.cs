using System;
using UnityEngine;
using TMPro;

public class timerManager : MonoBehaviour
{
    private TMP_Text _TimerText;
    enum TimerType {Countdown, Stopwatch}
    [SerializeField] private TimerType timerType;
    [SerializeField] private float TimeToDisplay;
    private bool _isRunning; //Si el timepo esta detenido es false
    // Start is called before the first frame update
    void Awake()
    {
        _TimerText=GetComponent<TMP_Text>();
    }
    private void OnEnable() {
        TimerEventManager.TimerStart += TimerEventManagerOnTimerStart;
        TimerEventManager.TimerStop += TimerEventManagerOnTimerStop;
        TimerEventManager.TimerUpdate += TimerEventManagerOnTimerUpdate;
    }

    void OnDisable()
    {
        TimerEventManager.TimerStart -= TimerEventManagerOnTimerStart;
        TimerEventManager.TimerStop -= TimerEventManagerOnTimerStop;
        TimerEventManager.TimerUpdate -= TimerEventManagerOnTimerUpdate;
    }
    private void TimerEventManagerOnTimerStart() => _isRunning = true;
    private void TimerEventManagerOnTimerStop() => _isRunning = false;

    private void TimerEventManagerOnTimerUpdate(float value) => TimeToDisplay += value;
    

    void Update()
    {
        if(!_isRunning) return; //Chequeo que este corriendo el tiempo
        if(timerType==TimerType.Countdown && TimeToDisplay < 0.0f){
            TimerEventManager.OnTimerStop();
            return;
        }; //Chequeo que no se acabo el tiempo
        TimeToDisplay += timerType == TimerType.Countdown ? -Time.deltaTime : TimeToDisplay;//actualizo el tiempo dependiendo de el tipo de conteo
    

        TimeSpan UptimeSpan = TimeSpan.FromSeconds(TimeToDisplay);//Utilizo TimeSpan para formatear el tiempo
        

        _TimerText.text = UptimeSpan.ToString(format:@"mm\:ss\:ff");

    }
}
