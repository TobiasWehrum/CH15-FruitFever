using System;
using UnityEngine;

[Serializable]
public class AudioClipWithVolume
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private float volume = 1f;
    //[SerializeField] private bool ignoreGlobalVolumeMultiplier;

    public AudioClip Clip { get { return clip; } }
    public float Volume { get { return volume; } }

    public bool Enabled
    {
        get { return true; }
    }

    //public bool IgnoreGlobalVolumeMultiplier { get { return ignoreGlobalVolumeMultiplier; } }
}
