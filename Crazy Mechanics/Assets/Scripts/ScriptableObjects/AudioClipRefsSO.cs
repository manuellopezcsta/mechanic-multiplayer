using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AudioClipRefsSO : ScriptableObject
{
    [Header("General")]
    public AudioClip[] delivery;
    public AudioClip[] endOfLevel;
    public AudioClip[] footstep;
    public AudioClip[] carEntering;
    public AudioClip[] carExiting;
    public AudioClip[] objectDrop;
    public AudioClip[] objectPickup;
    public AudioClip[] trash;

    [Header("Task")]
    public AudioClip[] oilDrain;
    public AudioClip[] oilAdd;
    public AudioClip[] diferencial;

    [Header("Disasters")]
    public AudioClip[] lightboxFixing;
    public AudioClip[] powerShutdown;
    public AudioClip[] mysteryBoxSpawn;
    public AudioClip[] mysteryBoxOpen;
    public AudioClip[] spawnOilSpills;
    public AudioClip[] discoNight;

    [Header("Tools")]
    public AudioClip[] cleaningOil;
    public AudioClip[] drill;
    public AudioClip[] hammer;
    public AudioClip[] cricket;
}
