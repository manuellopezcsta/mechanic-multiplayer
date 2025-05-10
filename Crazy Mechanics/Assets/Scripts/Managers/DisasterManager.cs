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

    // Para valores de Aceite
    [Header("Oil Spills")]
    [SerializeField] private List<Transform> oilSpawnPosition;
    [SerializeField] private GameObject oilPrefab;

    // Para disco Night
    [Header("Disco Night")]
    private LightManager lightManager;

    // Eventos que disparan los sonidos
    public static event EventHandler OnSpawnedMysteryBox;
    public static event EventHandler OnOilSpillsSpawned;
    public static event EventHandler OnDiscoNight;



    void Awake()
    {
        Instance = this;
        levelProperties = GameManager.Instance.GetLevelProperties();
        minTimer = levelProperties.disasterMinTimer;
        maxTimer = levelProperties.disasterMaxTimer;
        lightManager = GetComponent<LightManager>();
    }

    public enum DisasterType
    {
        LightBox, // Done
        Wind,
        OilStains, // Done
        MysteryBox, // Done
        DiscoNight // Done
    }
    // MysteryBox puede o invertir los controles, dar bufo de velocidad temporal, o darte dinero.

    void Update()
    {
        TriggerRandomDisaster();
    }

    // Disparador de desastres.
    private void TriggerRandomDisaster()
    {
        // Checkeamos si ya esta ocurriendo algun desastre.
        if (!disasterHappening)
        {
            // Generamos un time random, basado en el min y max del nivel
            int time = UnityEngine.Random.Range(minTimer, maxTimer);
            // Elegimos un num random de los disponibles.
            DisasterType type = (DisasterType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(DisasterType)).Length);
            if (levelProperties.disasterTypes.Contains(type))
            {
                StartCoroutine(WaitAndTriggerDisaster(time, type));
                disasterHappening = true;
            }
        }
    }

    IEnumerator WaitAndTriggerDisaster(int timer, DisasterType type)
    {
        Debug.Log("Iniciando Desastre: " + type + " en " + timer.ToString());
        yield return new WaitForSeconds(timer);

        switch (type)
        {
            case DisasterType.LightBox:
                disasterHappening = true;
                lightBoxController.CutDownPower();
                break;
            case DisasterType.Wind:
                disasterHappening = true;
                WindManager.Instance.TriggerWindEvent();
                break;
            case DisasterType.OilStains:
                TriggerOilSpillsDisaster();
                OnOilSpillsSpawned?.Invoke(this, EventArgs.Empty);
                break;
            case DisasterType.MysteryBox:
                Instantiate(mysteryBoxPrefab, mysteryBoxSpawner);
                OnSpawnedMysteryBox?.Invoke(this, EventArgs.Empty);
                break;
            case DisasterType.DiscoNight:
                disasterHappening = true;
                lightManager.TriggerDiscoNight();
                OnDiscoNight?.Invoke(this, EventArgs.Empty);
                break;
            default:
                Debug.LogWarning("ESTE ENUM NO EXISTE!, Disaster Manager");
                break;
        }
    }

    public void TriggerPlayerSpeedDisaster()
    {
        float normalSpeed = GameManager.playerList[0].speed;
        int modifier = Tools.GetOneOrMinusOne();
        // Si sale negativo , aplicamos debufo a la velocidad.
        if (modifier == -1)
        {
            foreach (Player player in GameManager.playerList)
            {
                player.speed = speedDebuffPower;
            }
        }
        else
        {
            // Sino le damos un bufo de velocidad.
            foreach (Player player in GameManager.playerList)
            {
                player.speed = speedBuffPower;
            }

        }
        Debug.Log("Se modifico la velocidad a " + GameManager.playerList[0].speed.ToString());
        StartCoroutine(ReturnPlayerToNormalSpeed(normalSpeed));
    }

    public void TriggerAddMoneyDisaster()
    {
        int money = UnityEngine.Random.Range(minMoney, maxMoney + 1);
        int modifier = Tools.GetOneOrMinusOne();
        Debug.Log("Agregando Puntos x Desastre: " + money.ToString());
        ScoreManager.Instance.AddPoints(money * modifier);
        disasterHappening = false;
    }

    public void TriggerInvertControlsDisaster()
    {
        Debug.Log("Invirtiendo Controles..");
        Player.invertControls = true;
        StartCoroutine(ReturnPlayerToNormalControls());
    }

    public void TriggerOilSpillsDisaster()
    {
        int ammount = UnityEngine.Random.Range(1, oilSpawnPosition.Count);
        List<int> shuffledIndexes = Tools.GetShuffledIndexes(oilSpawnPosition.Count);

        for (int i = 0; i < ammount; i++)
        {
            int index = shuffledIndexes[i];

            // Si ya existe un aceite aca, no lo spawneamos.
            if (oilSpawnPosition[index].childCount > 0)
            {
                continue;
            }

            // Spawneamos la mancha en la posicion elegida al azar de las disponibles
            GameObject oilSpill = Instantiate(oilPrefab, oilSpawnPosition[index]);
            oilSpill.transform.position = oilSpawnPosition[index].position;

            // Le damos una rotacion random.
            float randomYRotation = UnityEngine.Random.Range(0f, 360f); // Rotación aleatoria en Y
            Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f); //
            oilSpill.transform.rotation = randomRotation;
        }

        disasterHappening = false;
    }


    // En las funciones que devuelven todo a la normalidad , triggereamos disasterHappening a false cuando terminan.
    IEnumerator ReturnPlayerToNormalControls()
    {
        yield return new WaitForSeconds(invertedControlsDuration);
        Player.invertControls = false;
        disasterHappening = false;
    }

    IEnumerator ReturnPlayerToNormalSpeed(float newSpeed)
    {
        yield return new WaitForSeconds(speedBuffDuration);
        foreach (Player player in GameManager.playerList)
        {
            player.speed = newSpeed;
        }
        disasterHappening = false;
    }
}
