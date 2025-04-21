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
    public int PlayerIndex;
    [SerializeField] TextMeshProUGUI tittleText;
    [SerializeField] GameObject readyPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] private Button readyButton;

    private PlayerInput playerInput;
    private InputAction navigateAction; // Para el navegate.

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled;

    [Header("Renderer Cam")]
    [SerializeField] public Image imageDisplay; // UI Image que mostrará las imágenes
    public PlayerSelectContainerSO[] playerDataContainers;   // Lista de sprites de imágenes
    private int currentIndex = 0; // Índice actual de la imagen

    [SerializeField] private Vector3 posCamera;
    [SerializeField] private new Camera camera;
    [SerializeField] private RenderTexture[] renderCam;
    [SerializeField] private Material[] materialCamRenderer;
    [SerializeField] private Image imageRenderer;
 

private void Start() {
    //Le damos el prefab indicado al Player al iniciar el character select
    PlayerConfigurationManager.Instance.GetPlayerConfigs()[PlayerIndex].playerPrefab = playerDataContainers[currentIndex].playerPrefab;

    //Se le coloca la textura a la camara segun el player actual
    camera.targetTexture = renderCam[PlayerIndex];
    imageRenderer.material = materialCamRenderer[PlayerIndex];

    //Le sacamos el parent a la camara para trabajar con una unica vara de posiciones
    camera.transform.SetParent(null);
    posCamera = new Vector3(0f,1.2f,-7f);

    // colocamos la posicion inicial a la camara
    camera.transform.position = posCamera;
    
}
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
        navigateAction.started -= OnNavigate;
    }


    private void NextImage()
    {
        //Debug.Log("Se fue a la siguiente imagen");
        currentIndex++;
        //Variamos la posicion de la camara en conveniencia
        camera.transform.position += new Vector3(PlayerConfigurationManager.Instance.GetCameraOffset(),0f,0f);
        if (currentIndex >= playerDataContainers.Length)
        {
            currentIndex = 0;

            //Colocamos la posicion de la Camara frente al primer personaje disponible
            camera.transform.position = posCamera;
        }
        UpdateImage();
    }

    private void PreviousImage()
    {
        currentIndex--;
        //Variamos la posicion de la camara en conveniencia
        camera.transform.position -= new Vector3(PlayerConfigurationManager.Instance.GetCameraOffset(),0f,0f);

        if (currentIndex < 0)
        {
            currentIndex = playerDataContainers.Length - 1;

            //Colocamos la posicion de la Camara frente al ultimo personaje disponible
            camera.transform.position = new Vector3(PlayerConfigurationManager.Instance.GetCameraOffset() * currentIndex ,posCamera.y,posCamera.z);
        }
        UpdateImage();
    }

    private void UpdateImage( )
    {
        
        imageDisplay.sprite = playerDataContainers[currentIndex].sprite; // Actualiza la imagen mostrada
        // Updatemos los datos del player aca
        SetColor(playerDataContainers[currentIndex].material);
        //actualizar el prefab del personaje jugable
        PlayerConfigurationManager.Instance.GetPlayerConfigs()[PlayerIndex].playerPrefab = playerDataContainers[currentIndex].playerPrefab;

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

