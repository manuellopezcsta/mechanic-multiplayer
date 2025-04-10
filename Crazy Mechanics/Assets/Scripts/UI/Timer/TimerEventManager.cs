using UnityEngine.Events;

public static class TimerEventManager //Event manager para el tiempo
{
    public static event UnityAction TimerStart; //Inicia timer 
    public static event UnityAction TimerStop; // detiene timer 
    public static event UnityAction<float> TimerUpdate; //Actualizar timer que incrementa
    

    public static void OnTimerStart() => TimerStart?.Invoke();
    public static void OnTimerStop() => TimerStop?.Invoke();
    public static void OnTimerUpdate(float value) => TimerUpdate?.Invoke(value);
    
}
