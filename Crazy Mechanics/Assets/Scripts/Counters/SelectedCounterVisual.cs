using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private float detectionRadius = 2.3f; // NO TOCAR ESTE REWORK COSTO UN **** !
    private void Update()
    {
        bool isAnyPlayerClose = false;
        // Iterar sobre todos los jugadores en la escena
        foreach (Player player in GameManager.playerList)
        {
            // Verificar si el jugador está dentro del radio de detección
            if (Vector3.Distance(player.transform.position, transform.position) <= detectionRadius)
            {
                // Activar el visual si este BaseCounter es el seleccionado
                if (player.selectedCounter == baseCounter)
                {
                    isAnyPlayerClose = true; // Al menos 1 player esta cerca, activamos la visual.
                    break;
                }
            }
        }
        if (isAnyPlayerClose)
        {
            Show();
        }
        else
        {
            Hide();
        }

    }

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
