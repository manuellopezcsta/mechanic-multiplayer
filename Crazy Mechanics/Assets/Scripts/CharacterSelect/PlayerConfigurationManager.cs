using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigurationManager : MonoBehaviour
{
    [SerializeField] private List<PlayerConfiguration> playerConfigs;
    [SerializeField] private PlayerSelectContainerSO[] charactersVisuals;

    [SerializeField] private float offsetVisualCharacters;

    public static PlayerConfigurationManager Instance { get; private set; }

    
    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Singleton Already Exists");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            // Iniciamos la lista para guardar las configs.
            playerConfigs = new List<PlayerConfiguration>();
        }
    }
    public void Start(){
        RenderCharacters();
        GameManager.playerList.Clear();
    }

    // Le pasamos un player, y seteamos el material de ese player.
    public void SetPlayerColor(int index, Material color)
    {
        playerConfigs[index].PlayerMaterial = color;
    }

    // Setea el player para arrancar la partida.
    public void ReadyPlayer(int index)
    {
        playerConfigs[index].IsReady = true;
        // Chekeo si los players estan ready.
        if (playerConfigs.Count == playerConfigs.Count(p => p.IsReady == true))
        {
            // Cargamos la escena que corresponde
            Loader.Load(Loader.Scene.TestDemo);
        }
    }

    public void HandlePlayerJoined(PlayerInput pi)
    {
        // Si no a;adimos ya al player
        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            Debug.Log("Player " + pi.playerIndex + " Joined ");
            // Guardamos el obj input en el manager, para que persista cuando cambiamos la escena.
            pi.transform.SetParent(transform);
            // Agregamos un nuevo config con el indice de este pi.
            playerConfigs.Add(new PlayerConfiguration(pi));
            //Debug.Log(playerConfigs.Count);
        }
    }

    // Si no se usa en el futuro, borrar
    public void ClearPlayersPreFab()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<Player>(out Player player))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void SelfDestruct() {
        // CODIGO PARA DESUSCRIBIRSE AL DESTRUIRSE UN PLAYER.
        GameManager.NukePlayerControllers();
        Destroy(gameObject);
    }
    
    private void RenderCharacters(){
        Vector3 startingPosition = new Vector3(0f,0f,0f);
        Vector3 offset = new Vector3(offsetVisualCharacters,0f,0f);
        for (int i = 0; i < charactersVisuals.Length; i++){
            GameObject character = Instantiate(charactersVisuals[i].playerPrefab);
            character.transform.position = startingPosition;
            startingPosition += offset;
            character.GetComponent<Player>().enabled = false;
            character.GetComponent<PlayerInputHandler>().enabled = false;
            character.GetComponent<PlayerSound>().enabled = false;
            character.GetComponent<CharacterController>().enabled = false;
        }
    }
    public float GetCameraOffset(){
        return offsetVisualCharacters;
    }
}


public class PlayerConfiguration
{
    public PlayerInput PInput { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    // Cambiarlo x modelo si esto funciona
    public Material PlayerMaterial { get; set; }
    public GameObject playerPrefab { get; set; }

    // Constructor de la clase
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        PInput = pi;
    }
}
