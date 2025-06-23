using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanelUI : MonoBehaviour
{
    const string LEVEL_SELECT_SCENE = "WorldSelect";
    [SerializeField] Image[] throphies;
    [SerializeField] Button backButton;

    // Colores de los trofeos
    [SerializeField] Color bronzeColor;
    [SerializeField] Color silverColor;
    [SerializeField] Color goldColor;

    [SerializeField] TextMeshProUGUI scoreText;
    int levelScore;

    void Start()
    {
        Hide();
        backButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.WorldSelect);
            Time.timeScale = 1f;

            // CODIGO ALTAMENTE INESTABLE... NO TOCAR!!!! MANU APPROVED THIS.
            //PlayerConfigurationManager.Instance.SwitchInputMethod(false);
            GameManager.NukePlayerControllers();
        });
    }

    public void ShowThrophies()
    {
        LevelProperties levelProperties = GameManager.Instance.GetLevelProperties();
        // Obtenemos el score del jugador y los score a vencer
        levelScore = (int)ScoreManager.Instance.GetScore();
        int firstStarScore = levelProperties.firstStarScore;
        int secondStarScore = levelProperties.secondStarScore;
        int thirdStarScore = levelProperties.thirdStarScore;

        // Pintamos las estrellas dependiendo del score del player.
        // Y guardamos en un playerPrefs x si lo queremos usar despues en algun lado onda score a vencer etc.
        if (levelScore >= thirdStarScore)
        {
            SetThophiesColors(3, goldColor);
            SetPlayerPrefStarsForLevel(levelProperties.levelNumber, 3);
        }
        else if (levelScore >= secondStarScore)
        {
            SetThophiesColors(2, silverColor);
            SetPlayerPrefStarsForLevel(levelProperties.levelNumber, 2);
        }
        else if (levelScore >= firstStarScore)
        {
            SetThophiesColors(1, bronzeColor);
            SetPlayerPrefStarsForLevel(levelProperties.levelNumber, 1);
        }
        else
        {
            SetThophiesColors(0, bronzeColor);
            SetPlayerPrefStarsForLevel(levelProperties.levelNumber, 0);
        }

        // Guardamos el HighScore si corresponde.
        SetPlayerPrefScoreForLevel(levelProperties.levelNumber);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        backButton.Select();
        Time.timeScale = 0f;
        ShowThrophies();
        scoreText.text = levelScore.ToString();
    }

    // Guarda las estrellas por nivel.
    private void SetPlayerPrefStarsForLevel(string levelNumber, int starsToSet)
    {
        if (PlayerPrefs.GetInt(levelNumber, 0) <= starsToSet)
        {
            PlayerPrefs.SetInt(levelNumber, starsToSet);
        }
    }

    // Guarda el HighScore del Level de ser Necesario
    private void SetPlayerPrefScoreForLevel(string levelNumber)
    {
        // Cargamos el maximo anterior
        int scoreToBeat = PlayerPrefs.GetInt(levelNumber.ToString() + "Score", 0);

        // Si nuestro score paso el Max lo guardamos.
        if (levelScore > scoreToBeat)
        {
            PlayerPrefs.SetInt(levelNumber.ToString() + "Score", levelScore);
        }
    }

    private void SetThophiesColors(int quantity, Color color)
    {
        if(quantity == 0) { return; }
        for (int i = 0; i < quantity; i++)
        {
            throphies[i].gameObject.SetActive(true);
            throphies[i].color = color;
        }
    }
}
