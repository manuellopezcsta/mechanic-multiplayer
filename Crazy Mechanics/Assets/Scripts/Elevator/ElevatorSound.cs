using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSound : MonoBehaviour
{
    [SerializeField] private ElevatorController controller;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        controller.OnMovingChanged += OnMovingChanged;   
    }

    private void OnMovingChanged(object sender, ElevatorController.OnMovingChangedEventArgs e)
    {
        float volume = SoundManager.Instance.GetSfxVolume();
        audioSource.volume = volume;

        if(e.isMoving) {
            audioSource.Play();
        } else {
            audioSource.Stop();
        }
    }
}
