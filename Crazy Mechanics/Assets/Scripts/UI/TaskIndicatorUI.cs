using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskIndicatorUI : MonoBehaviour
{
    //Referencias para prender apagar y cambiar la visual del progressTask
    [SerializeField] private Image taskIcon;
    [SerializeField] private Sprite completedTaskIcon;
    [SerializeField] private Sprite missingWheelSprite;

    public void SetAsComplete()
    {
        taskIcon.sprite = completedTaskIcon;
        StartCoroutine(WaitAndHIde(3f));
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    IEnumerator WaitAndHIde(float timeRequest)
    {
        yield return new WaitForSeconds(timeRequest);
        Hide();
    }

    public void SwapToMissingWheelIcon()
    {
        taskIcon.sprite = missingWheelSprite;
    }
}
