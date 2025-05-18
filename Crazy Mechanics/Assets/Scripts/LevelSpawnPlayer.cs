using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawnPlayer : MonoBehaviour
{
    private Transform[] playerSpawns;
    //[SerializeField] private GameObject playerPrefab;

    void Start() {
        playerSpawns = GameManager.Instance.playerSpawns;
        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
        for (int i = 0; i < playerConfigs.Length; i++)
        {
            //Instanciamos el player con su personaje seleccionado.
            var player = Instantiate(playerConfigs[i].playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
            player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i]);
            playerConfigs[i].PInput.defaultActionMap = "Player";
            //Debug.Log(playerConfigs.Length);
            Debug.Log(playerConfigs[i].PlayerIndex);
        }
    }
}
