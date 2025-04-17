using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    const string SCENE_NAME = "Test Demo";
    private List<PlayerConfiguration> playerConfigs;

    [SerializeField] private int MaxPlayers = 4;

    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Singleton Already Exists");
        } else {
            Instance = this;
            DontDestroyOnLoad(Instance);
            // Iniciamos la lista para guardar las configs.
            playerConfigs = new List<PlayerConfiguration>();
        }
    }

    // Le pasamos un player, y seteamos el material de ese player.
    public void SetPlayerColor(int index, Material color)
    {
        playerConfigs[index].PlayerMaterial = color;
    }

    // Setea el player para arrancar la partida.
    public void ReadyPlayer(int index) {
        playerConfigs[index].IsReady = true;
        // Chekeo si los players estan ready.
        if(playerConfigs.Count == playerConfigs.Count(p => p.IsReady == true)) {
            // Cargamos la escena que corresponde
            SceneManager.LoadScene(SCENE_NAME);
        }
    }

    public void HandlePlayerJoined(PlayerInput pi) {
        Debug.Log("Player Joined " + pi.playerIndex);
        // Si no a;adimos ya al player
        if(!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex)) {
            // Guardamos el obj input en el manager, para que persista cuando cambiamos la escena.
            pi.transform.SetParent(transform);
            // Agregamos un nuevo config con el indice de este pi.
            playerConfigs.Add(new PlayerConfiguration(pi));
        }
    }
}


public class PlayerConfiguration
{
    public PlayerInput PInput { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    // Cambiarlo x modelo si esto funciona
    public Material PlayerMaterial {get; set; }

    // Constructor de la clase
    public PlayerConfiguration(PlayerInput pi) {
        PlayerIndex = pi.playerIndex;
        PInput = pi;
    }
}
