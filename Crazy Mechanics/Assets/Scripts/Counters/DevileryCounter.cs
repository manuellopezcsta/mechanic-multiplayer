using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



public class DevileryCounter : BaseCounter
{
    public static DevileryCounter Instance { get; private set; }

    [SerializeField] private GameObject fxMoney;

    private CurrentStationManager[] listElevators;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        listElevators = GameManager.Instance.stations;
    }
    public override void Interact(Player player)
    {
        foreach (var currentStation in listElevators){
            currentStation.TryToDeliverCar();
            //Debug.Log("Se entrego algo");
        }
    }

    public void FxMoneyActive()
    {
        fxMoney.SetActive(true);
        StartCoroutine(ShutdownFxWhenFinalize());
    }

    public IEnumerator ShutdownFxWhenFinalize()
    {
        var particle = fxMoney.GetComponent<ParticleSystem>();
        if (particle != null)
        {
            // Espera a que termine la animación de partículas
            yield return new WaitWhile(() => particle.isPlaying);
        }
        // Desactiva el objeto para que pueda volver a activarse la próxima vez
        fxMoney.SetActive(false);
    }
}

    // Por ahi con el alternateInteract, podemos elegir cual queremos entregar ?
