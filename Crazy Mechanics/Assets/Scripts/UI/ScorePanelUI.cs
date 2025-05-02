using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanelUI : MonoBehaviour
{
    const string LEVEL_SELECT_SCENE = "WorldSelect";
    [SerializeField] Image stars;
    [SerializeField] Button backButton;

    void Start() {
        Hide();
        backButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.WorldSelect);
            Time.timeScale = 1f;

            // CODIGO ALTAMENTE INESTABLE... NO TOCAR!!!! MANU APPROVED THIS.
            PlayerConfigurationManager.Instance.SwitchInputMethod(false);
            GameManager.NukePlayerControllers();
        });
    }

    public void ShowStars() {
        float fillAmountFor1Star = 0.35f;
        float fillAmountFor2Star = 0.68f;

        LevelProperties levelProperties = GameManager.Instance.GetLevelProperties();
        // Obtenemos el score del jugador y los score a vencer
        int levelScore = ScoreManager.Instance.GetScore();
        int firstStarScore = levelProperties.firstStarScore;
        int secondStarScore = levelProperties.secondStarScore;
        int thirdStarScore = levelProperties.thirdStarScore;

        // Pintamos las estrellas dependiendo del score del player.
        // Y guardamos en un playerPrefs x si lo queremos usar despues en algun lado onda score a vencer etc.
        if(levelScore >= thirdStarScore) {
            stars.fillAmount = 1f;
            SetPlayerPrefScoreForLevel(levelProperties.levelNumber, 3);
        } else if (levelScore >= secondStarScore) {
            stars.fillAmount = fillAmountFor2Star;
            SetPlayerPrefScoreForLevel(levelProperties.levelNumber, 2);
        } else if ( levelScore >= firstStarScore) {
            stars.fillAmount = fillAmountFor1Star;
            SetPlayerPrefScoreForLevel(levelProperties.levelNumber, 1);
        } else {
            stars.fillAmount = 0f;
            SetPlayerPrefScoreForLevel(levelProperties.levelNumber, 0);
        }
    }

    void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
        backButton.Select();
        Time.timeScale = 0f;
        ShowStars();
    }

    private void SetPlayerPrefScoreForLevel(string levelNumber, int starsToSet){
        if(PlayerPrefs.GetInt(levelNumber, 0) <= starsToSet) {
            PlayerPrefs.SetInt(levelNumber, starsToSet);
        }
    }
}
