using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AudioClipRefsSO : ScriptableObject
{
    public AudioClip[] delivery;
    public AudioClip[] footstep;
    public AudioClip[] objectDrop;
    public AudioClip[] objectPickup;
    public AudioClip[] trash;
    public AudioClip[] carEntering;
    public AudioClip[] carExiting;
    public AudioClip[] oilDrain;
    public AudioClip[] oilAdd;
    public AudioClip[] lightboxFixing;
    public AudioClip[] endOfLevel;
}
