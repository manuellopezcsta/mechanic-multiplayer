using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    public GameObject playerSetupMenuPrefab;
    public PlayerInput input;
    const string ROOT_MENU_GAME_OBJECT_NAME = "MainLayout1";

    void Awake()
    {
        GameObject rootMenu = GameObject.Find(ROOT_MENU_GAME_OBJECT_NAME);
        if(rootMenu != null) {
            // Creo un menu de seleccion para el player 1.
            GameObject menu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
            // Asigno el input del player al input de ese menu. ????
            input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
            menu.GetComponent<PlayerSetUpMenuController>().SetPlayerIndex(input.playerIndex);
            //Codigo experimental.
            menu.GetComponent<PlayerSetUpMenuController>().SetPlayerInput(input);
        }
    }
}
