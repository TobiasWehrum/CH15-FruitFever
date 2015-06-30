using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings : PersistentSingletonMonoBehaviour<Settings>
{
    public event Action<bool> EventMusicChanged;
    public event Action<bool> EventSfxChanged;
    public event Action EventAmplitudeChanged;
    public event Action EventTransparencyChanged;
    public event Action EventOrgansChanged;

    [SerializeField] private bool saveSettings;
    [SerializeField] private bool sfx;
    [SerializeField] private bool music;
    [SerializeField] private int amplitude = 3;
    [SerializeField] private int transparency = 4;
    [SerializeField] private int organs = 3;

    private int seed;

    protected override void OnFirstLoad()
    {
        if (saveSettings)
        {
            sfx = UnityHelper.PlayerPrefsGetBool("Sfx", Sfx);
            music = UnityHelper.PlayerPrefsGetBool("Music", Music);
            amplitude = PlayerPrefs.GetInt("Amplitude", Amplitude);
            transparency = PlayerPrefs.GetInt("Transparency", Transparency);
            organs = PlayerPrefs.GetInt("Organs", Organs);
        }

        seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            var newTransparency = Math.Min(Transparency, Organs + 1) - 1;
            if (newTransparency < 0)
                newTransparency = 4;

            Transparency = newTransparency;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            var newOrgans = Organs + 1;
            if (newOrgans > 3)
            {
                newOrgans = 1;
                Transparency = Mathf.Max(Transparency - 2, 0);
            }
            else
            {
                if (Transparency > 0)
                    Transparency++;
            }

            Organs = newOrgans;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Amplitude = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Amplitude = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Amplitude = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Amplitude = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Amplitude = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Amplitude = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Amplitude = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Amplitude = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Amplitude = 9;
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

    public int Amplitude
    {
        get { return amplitude; }
        set
        {
            if (amplitude == value)
                return;

            amplitude = value;

            PlayerPrefs.SetInt("Amplitude", Amplitude);

            if (EventAmplitudeChanged != null)
                EventAmplitudeChanged();
        }
    }

    public int Transparency
    {
        get { return transparency; }
        set
        {
            if (transparency == value)
                return;

            transparency = value;

            PlayerPrefs.SetInt("Transparency", Transparency);

            if (EventTransparencyChanged != null)
                EventTransparencyChanged();
        }
    }

    public int Organs
    {
        get { return organs; }
        set
        {
            if (organs == value)
                return;

            organs = value;

            PlayerPrefs.SetInt("Organs", Organs);

            if (EventOrgansChanged != null)
                EventOrgansChanged();
        }
    }

    public int Seed
    {
        get { return seed; }
        set { seed = value; }
    }
}
