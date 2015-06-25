using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings : PersistentSingletonMonoBehaviour<Settings>
{
    public event Action<bool> EventMusicChanged;
    public event Action<bool> EventSfxChanged;
    public event Action<int> EventDifficultyChanged;

    [SerializeField] private bool saveSettings;
    [SerializeField] private bool sfx;
    [SerializeField] private bool music;
    [SerializeField] private int difficulty = 3;

    protected override void OnFirstLoadOrSwitch()
    {
        if (saveSettings)
        {
            sfx = UnityHelper.PlayerPrefsGetBool("Sfx", Sfx);
            music = UnityHelper.PlayerPrefsGetBool("Music", Music);
            difficulty = PlayerPrefs.GetInt("Difficulty", Difficulty);
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Difficulty = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Difficulty = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Difficulty = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Difficulty = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Difficulty = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Difficulty = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Difficulty = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Difficulty = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Difficulty = 9;
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

    public int Difficulty
    {
        get { return difficulty; }
        set
        {
            if (difficulty == value)
                return;

            difficulty = value;

            PlayerPrefs.SetInt("Difficulty", Difficulty);

            if (EventDifficultyChanged != null)
                EventDifficultyChanged(difficulty);
        }
    }
}
