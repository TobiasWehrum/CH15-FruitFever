using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings : PersistentSingletonMonoBehaviour<Settings>
{
    public event Action<bool> EventMusicChanged;
    public event Action<bool> EventSfxChanged;

    [SerializeField] private bool saveSettings;
    [SerializeField] private bool sfx;
    [SerializeField] private bool music;

    protected override void OnFirstLoadOrSwitch()
    {
        if (saveSettings)
        {
            sfx = UnityHelper.PlayerPrefsGetBool("Sfx", Sfx);
            music = UnityHelper.PlayerPrefsGetBool("Music", Music);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Sfx = !Sfx;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Music = !Music;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    public bool Music
    {
        get { return music; }
        set
        {
            if (music == value)
                return;

            music = value;

            UnityHelper.PlayerPrefsSetBool("Music", Music);

            if (EventMusicChanged != null)
                EventMusicChanged(music);
        }
    }

    public bool Sfx
    {
        get { return sfx; }
        set
        {
            if (sfx == value)
                return;

            sfx = value;

            UnityHelper.PlayerPrefsSetBool("Sfx", Sfx);

            if (EventSfxChanged != null)
                EventSfxChanged(sfx);
        }
    }
}
