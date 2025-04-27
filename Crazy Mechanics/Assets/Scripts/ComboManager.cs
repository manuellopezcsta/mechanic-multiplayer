using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{

    public static ComboManager Instance { get; private set; }


    private int comboCount = 0; // Contador de combos
    [SerializeField] private float comboTimer = 0f; // Temporizador de combo
    public float comboResetTime = 2f; // Tiempo antes de reiniciar combo si no se llama a Interact

    [SerializeField] public int multiplierCombo = 1;
    [SerializeField] private float multiplierCounter = 0;
    [SerializeField] private float maxMultiplierCounter = 5;

    [SerializeField] private Image visualCounterCombo;
    [SerializeField] private TextMeshProUGUI visualCombo;

    private void Awake()
    {
        Instance = this;
        UpdateVisual();
    }
    // Método que se llama en cada frame
    void Update()
    {
        // Reducir el temporizador del combo con el tiempo
        if (comboCount > 0)
        {
            comboTimer -= Time.deltaTime;

            // Reiniciar combo si el tiempo se acaba
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    // Función que se llama cuando se activa Interact
    public void UpdateCombo()
    {
        comboCount++;

        // Incrementar combo
        changeValueMultiplier();
        //Debug.Log("Se aumento el comboCounter");

        // Reiniciar temporizador del combo
        comboTimer = comboResetTime;
    }

    // Función para reiniciar el combo

    private void ResetCombo()
    {
        comboCount = 0;

        multiplierCombo = 1;
        multiplierCounter = 0;
        UpdateVisual();
    }
    private void changeValueMultiplier()
    {
        multiplierCounter++;
        if (multiplierCounter >= maxMultiplierCounter)
        {
            //Le sumamos uno al multiplicador de combo y lo limitamos a un max de 3
            multiplierCombo = Math.Min(multiplierCombo + 1, 3);
            multiplierCounter = 0;
        }
        //Debug.Log(multiplierCounter);
        UpdateVisual();
    }

    private void UpdateVisual() {
     
        float counterCombo = (1 / maxMultiplierCounter) * multiplierCounter;
        //Debug.Log(counterCombo);
        visualCounterCombo.fillAmount = counterCombo;
        visualCombo.text = multiplierCombo.ToString();
    }

}