using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEditor;
using UnityEngine;

public class DisasterManager : MonoBehaviour
{
    public static DisasterManager Instance { get; private set; }
    LevelProperties levelProperties;
    int minTimer;
    int maxTimer;
    public bool disasterHappening;

    [SerializeField] private LightBoxController lightBoxController;
    [SerializeField] private Transform mysteryBoxSpawner;
    [SerializeField] private GameObject mysteryBoxPrefab;

    // Valores de bufos
    [Header("Buff values")]
    [SerializeField] private float invertedControlsDuration = 10f;
    [SerializeField] private float speedBuffDuration = 10f;
    [SerializeField] private float speedBuffPower = 10f;
    [SerializeField] private float speedDebuffPower = 4f;
    [SerializeField] private int minMoney = 20;
    [SerializeField] private int maxMoney = 40;

    // Eventos que disparan los sonidos
    public static event EventHandler OnSpawnedMysteryBox;
    

    void Awake()
    {
        Instance = this;
        levelProperties = GameManager.Instance.GetLevelProperties();
        minTimer = levelProperties.disasterMinTimer;
        maxTimer = levelProperties.disasterMaxTimer;
    }

    public enum DisasterType {
        LightBox, // Done
        Wind,
        OilStains,
        MysteryBox, // Done
        DiscoNight
    }
    // MysteryBox puede o invertir los controles, dar bufo de velocidad temporal, o darte dinero.

    void Update()
    {
        TriggerRandomDisaster();
    }

    // Disparador de desastres.
    private void TriggerRandomDisaster() {
        // Checkeamos si ya esta ocurriendo algun desastre.
        if (!disasterHappening) {
            // Generamos un time random, basado en el min y max del nivel
            int time = UnityEngine.Random.Range(minTimer, maxTimer);
            // Elegimos un num random de los disponibles.
            DisasterType type = (DisasterType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(DisasterType)).Length);
            StartCoroutine(WaitAndTriggerDisaster(time, type));
            disasterHappening = true;
        }
    }

    IEnumerator WaitAndTriggerDisaster(int timer, DisasterType type) {
        Debug.Log("Iniciando Desastre: " + type + " en " + timer.ToString());
        yield return new WaitForSeconds(timer);

        switch (type) {
            case DisasterType.LightBox:
                lightBoxController.CutDownPower();
                break;
            case DisasterType.Wind:
                break;
            case DisasterType.OilStains:
                break;
            case DisasterType.MysteryBox:
                Instantiate(mysteryBoxPrefab, mysteryBoxSpawner);
                OnSpawnedMysteryBox?.Invoke(this, EventArgs.Empty);
                break;
            case DisasterType.DiscoNight:
                break;
            default:
                Debug.LogWarning("ESTE ENUM NO EXISTE!, disaster manager");
                break;
        }
        // Se tiene que desactivar cuando se termina un desastre, no aca.
        disasterHappening = true;
    }

    public void TriggerPlayerSpeedDisaster() {
        float normalSpeed = Player.speed;
        int modifier = Tools.GetOneOrMinusOne();
        // Si sale negativo , aplicamos debufo a la velocidad.
        if(modifier == -1)
        {
            Player.speed = speedDebuffPower;
        } else {
            // Sino le damos un bufo de velocidad.
            Player.speed = speedBuffPower;
        }
        Debug.Log("Se modifico la velocidad a " + Player.speed.ToString());
        StartCoroutine(ReturnPlayerToNormalSpeed(normalSpeed));
    }

    public void TriggerAddMoneyDisaster() {
        int money = UnityEngine.Random.Range(minMoney, maxMoney + 1);
        int modifier = Tools.GetOneOrMinusOne();
        Debug.Log("Agregando Puntos x Desastre: " + money.ToString());
        ScoreManager.Instance.AddPoints(money * modifier);
        disasterHappening = false;
    }

    public void TriggerInvertControlsDisaster() {
        Debug.Log("Invirtiendo Controles..");
        Player.invertControls = true;
        StartCoroutine(ReturnPlayerToNormalControls());
    }


    // En las funciones que devuelven todo a la normalidad , triggereamos disasterHappening a false cuando terminan.
    IEnumerator ReturnPlayerToNormalControls() {
        yield return new WaitForSeconds(invertedControlsDuration);
        Player.invertControls = false;
        disasterHappening = false;
    }

    IEnumerator ReturnPlayerToNormalSpeed(float newSpeed) {
        yield return new WaitForSeconds(speedBuffDuration);
        Player.speed = newSpeed;
        disasterHappening = false;
    }
}
