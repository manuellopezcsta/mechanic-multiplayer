using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancingMachineAudio : MonoBehaviour
{
    [SerializeField] private BalacingTool balacingTool;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        balacingTool.OnStateChanged += BalacingTool_OnStateChanged;   
    }

    private void BalacingTool_OnStateChanged(object sender, BalacingTool.OnStateChangedEventArgs e)
    {
        float volume = SoundManager.Instance.GetSfxVolume();
        audioSource.volume = volume;
        bool playSound = e.state == BalacingTool.State.Running;

        if(playSound) {
            audioSource.Play();
        } else {
            audioSource.Stop();
        }
    }
}
