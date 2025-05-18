using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameImputWorldSelect : MonoBehaviour
{
    private PlayerInput playerInput;
    void Start()
    {
        var playerConfig = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray()[0];
    }


}
