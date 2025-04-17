using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// https://youtu.be/_5pOiYHJgl0?t=2370
// Ta roto el input arreglar.

public class PlayerSetUpMenuController : MonoBehaviour
{
    private int PlayerIndex;
    [SerializeField] TextMeshProUGUI tittleText;
    [SerializeField] GameObject readyPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] private Button readyButton;

    private PlayerInput playerInput;
    private InputAction navigateAction; // Para el navegate.

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled;


    [SerializeField] public Image imageDisplay; // UI Image que mostrará las imágenes
    public PlayerSelectContainerSO[] playerDataContainers;   // Lista de sprites de imágenes
    private int currentIndex = 0; // Índice actual de la imagen




    // Seteamos el player Input para poder hacer el evento de callback de navigate.
    public void SetPlayerInput(PlayerInput pi)
    {
        playerInput = pi;
        //Obtenemos la accion de navegate
        navigateAction = playerInput.actions["Navigate"];
        // Suscribirse al evento de navegación
        navigateAction.started += OnNavigate;
    }

    // Maneja el input al presionar derecha e izquierda de los controller.
    private void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (input.x > 0) // Joystick hacia la derecha
        {
            NextImage();
        }
        else if (input.x < 0) // Joystick hacia la izquierda
        {
            PreviousImage();
        }
        // SI se bugea subir a 0.3 la deadzone de Navigate
    }

    private void OnDestroy()
    {
        // Asegurarse de desuscribirse del evento
        navigateAction.performed -= OnNavigate;
    }


    private void NextImage()
    {
        Debug.Log("Se fue a la siguiente imagen");
        currentIndex++;
        if (currentIndex >= playerDataContainers.Length)
        {
            currentIndex = 0;
        }
        UpdateImage();
    }

    private void PreviousImage()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = playerDataContainers.Length - 1;
        }
        UpdateImage();
    }

    private void UpdateImage()
    {
        imageDisplay.sprite = playerDataContainers[currentIndex].sprite; // Actualiza la imagen mostrada
        // Updatemos los datos del player aca
        SetColor(playerDataContainers[currentIndex].material);
    }

    /////////////////////////
    public void SetPlayerIndex(int pi)
    {
        PlayerIndex = pi;
        tittleText.SetText("Player " + (pi + 1).ToString());
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    void Update()
    {
        // Si paso el tiempo minimo, habilitamos los inputs.
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    // Para cambiar los colores al empezar a jugar.
    public void SetColor(Material color)
    {
        if (!inputEnabled)
        {
            return;
        }

        PlayerConfigurationManager.Instance.SetPlayerColor(PlayerIndex, color);

        readyButton.Select();

    }

    public void ReadyPlayer()
    {
        if (!inputEnabled)
        {
            return;
        }

        PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);
        readyButton.gameObject.SetActive(false);
        menuPanel.SetActive(false);
        readyPanel.SetActive(true);
        navigateAction.Disable();
    }
}

