using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Importar el nuevo sistema de input

public class IntroGame : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float holdTime = 3f;
    private float elapsedTime = 0f;
    private bool isHolding = false;
    private Image holdSkip;
    private float valueHold;
    public InputAction holdAction; // Crear una acción de input

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        holdSkip = GetComponent<Image>();

        holdAction.Enable(); // Habilitar la acción de input
    }

    void Update()
    {
        if (holdAction.IsPressed()) // Detectar si el botón está presionado
        {
            isHolding = true;
            elapsedTime += Time.deltaTime;
            valueHold = 1f - (elapsedTime / holdTime);
            holdSkip.fillAmount = valueHold;

            if (elapsedTime >= holdTime)
            {
                SceneManager.LoadScene("MenuNuevo");
            }
        }
        else
        {
            isHolding = false;
            elapsedTime = 0f;
            holdSkip.fillAmount = 1f;
        }
    }

    void OnVideoEnd(VideoPlayer video)
    {
        SceneManager.LoadScene("MenuNuevo");
    }
}