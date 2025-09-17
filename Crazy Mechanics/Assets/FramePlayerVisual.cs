
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FramePlayerVisual : MonoBehaviour
{
    public static FramePlayerVisual Instance { get; private set; }
    public List<GameObject> playerVisual = new List<GameObject>();
    public List<Sprite> playerVisualSprites = new List<Sprite>();
    public List<PlayerConfiguration> listConfigs = new List<PlayerConfiguration>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        List<PlayerConfiguration> listConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs();
        playerVisualSprites = PlayerConfigurationManager.Instance.playerVisualSprites;
        for (int i = 0; i < listConfigs.Count; i++)
        {
            playerVisual[i].GetComponent<Image>().sprite = playerVisualSprites[i];
        }
    }
    public void SetUpVisualPlayer()
    {
        List<PlayerConfiguration> listConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs();
        playerVisualSprites = PlayerConfigurationManager.Instance.playerVisualSprites;
        for (int i = 0; i < listConfigs.Count; i++)
        {
            playerVisual[i].GetComponent<Image>().sprite = playerVisualSprites[i];
        }
    }
}
