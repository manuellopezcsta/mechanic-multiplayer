using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class VideoButton : MonoBehaviour, ISelectHandler
{
    public string videoName;
    public VideoClip video;
    public TutorialUIManager videoUI;

    public void OnSelect(BaseEventData eventData)
    {
        videoUI.ChangeVideo(video, videoName);
    }
}

