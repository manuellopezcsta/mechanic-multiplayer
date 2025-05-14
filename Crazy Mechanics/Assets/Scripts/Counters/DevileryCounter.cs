using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



public class DevileryCounter : BaseCounter
{
    public static DevileryCounter Instance { get; private set; }

    [SerializeField] private GameObject moneyParticleGO;

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

    public void ShowMoneyParticles()
    {
        moneyParticleGO.SetActive(true);
        StartCoroutine(ShutdownParticlesWhenDone());
    }

    public IEnumerator ShutdownParticlesWhenDone()
    {
        var particle = moneyParticleGO.GetComponent<ParticleSystem>();
        if (particle != null)
        {
            // Espera a que termine la animaci�n de part�culas
            yield return new WaitWhile(() => particle.isPlaying);
        }
        // Desactiva el objeto para que pueda volver a activarse la pr�xima vez
        moneyParticleGO.SetActive(false);
    }
}

    // Por ahi con el alternateInteract, podemos elegir cual queremos entregar ?
