using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SfxManagerBase<T> : SingletonMonoBehaviour<T> where T : SfxManagerBase<T>
{
    [SerializeField] private int startOneShotAudioSources = 1;
    [SerializeField] private float globalVolumeMultiplier = 0.1f;
    [SerializeField] private float defaultSoundCooldown = 0.1f;
    [SerializeField] private float panMax = 0.5f;

    private AudioListener audioListener;

    private List<AudioSource> oneShotAudioSources;

    private Settings settings;

    private List<AudioClip> clipsInSoundCooldowns;
    private Dictionary<AudioClip, float> soundCooldowns;

    public bool Muted
    {
        get { return (settings != null) && (!settings.Sfx); }
    }

    private void Awake()
    {
        clipsInSoundCooldowns = new List<AudioClip>();
        soundCooldowns = new Dictionary<AudioClip, float>();
        oneShotAudioSources = new List<AudioSource>();

        settings = Settings.Instance;

        if (settings != null)
            settings.EventSfxChanged += OnSfxVolumeChanged;

        UpdateMuteSettings();

        for (int i = 0; i < startOneShotAudioSources; i++)
        {
            CreateTemporaryAudioSource();
        }
    }

    private void Start()
    {
        audioListener = UnityHelper.FindFirstActiveInstance<AudioListener>();
    }

    private void OnDestroy()
    {
        if (settings != null)
            settings.EventSfxChanged -= OnSfxVolumeChanged;
    }

    private void Update()
    {
        foreach (var clip in clipsInSoundCooldowns)
        {
            soundCooldowns[clip] -= Time.deltaTime;
        }
    }

    private void OnSfxVolumeChanged(bool sfxActive)
    {
        UpdateMuteSettings();
    }

    private void UpdateMuteSettings()
    {
        foreach (var audioSource in oneShotAudioSources)
        {
            audioSource.mute = Muted;
        }
    }

    // ----- Helper methods -----

    public void PlaySoundDelayed(AudioClipWithVolume sound, float delay, float xPosition)
    {
        if (delay <= 0)
        {
            PlaySound(sound, xPosition);
        }
        else
        {
            StartCoroutine(PlaySoundDelayedCoroutine(sound, delay, xPosition));
        }
    }

    public IEnumerator PlaySoundDelayedCoroutine(AudioClipWithVolume sound, float delay, float xPosition)
    {
        yield return new WaitForSeconds(delay);
        PlaySound(sound, xPosition);
    }

    public void PlaySound(AudioClipWithVolume[] audioClips, int index, float xPosition)
    {
        if ((index < 0) || (index >= audioClips.Length))
            return;

	    PlaySound(audioClips[index], xPosition);
    }

    public void PlaySound(AudioClipWithVolume[] audioClips, float xPosition)
    {
        if ((audioClips == null) || (audioClips.Length == 0))
            return;

        var audioClip = audioClips.RandomElement();
        if (((audioClip == null) || !audioClip.Enabled) && audioClips.Any(clip => (clip != null) && clip.Enabled))
        {
            audioClip = audioClips.Where(clip => (clip != null) && clip.Enabled).ToArray().RandomElement();
        }
        PlaySound(audioClip, xPosition);
    }

    public void PlaySound(AudioClipWithVolume audioClip, float xPosition)
    {
        if ((audioClip == null) || !audioClip.Enabled)
            return;

        var pitch = 1f;
        PlaySound(audioClip.Clip, audioClip.Volume, true, pitch, xPosition);
    }

    public void PlaySound(AudioClip[] audioClips, float volume, float pitch, float xPosition)
    {
        if ((audioClips == null) || (audioClips.Length == 0))
            return;

        var audioClip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];

        PlaySound(audioClip, volume, true, pitch, xPosition);
    }

    public void PlaySound(AudioClip audioClip, float volume, bool useGlobalVolumeMultiplier, float pitch, float xPosition)
    {
        if (audioListener == null)
            return;

        if (Muted)
            return;

        if (audioClip == null)
            return;

        if (defaultSoundCooldown > 0)
        {
            if (clipsInSoundCooldowns.Contains(audioClip))
            {
                if (soundCooldowns[audioClip] > 0)
                    return;
            }
            else
            {
                clipsInSoundCooldowns.Add(audioClip);
            }

            soundCooldowns[audioClip] = defaultSoundCooldown;
        }


        var position = audioListener.transform.position;

        if (useGlobalVolumeMultiplier)
            volume *= globalVolumeMultiplier;

        //AudioSource.PlayClipAtPoint(audioClip, position, volume);

        var audioSource = GetOrCreateTemporaryAudioSource();
        audioSource.clip = audioClip;
        audioSource.transform.position = position;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        if (xPosition == 0f)
        {
            audioSource.panStereo = 0f;
        }
        else
        {
            audioSource.panStereo = xPosition;
        }

        audioSource.Play();
    }

    public AudioSource CreateTemporaryAudioSource()
    {
        var newGameObject = new GameObject("Temporary Audio Source #" + (oneShotAudioSources.Count + 1));
        newGameObject.transform.parent = transform;

        var audioSource = newGameObject.AddComponent<AudioSource>();

        oneShotAudioSources.Add(audioSource);

        return audioSource;
    }

    public AudioSource GetOrCreateTemporaryAudioSource()
    {
        foreach (var audioSource in oneShotAudioSources)
        {
            if (!audioSource.isPlaying)
                return audioSource;
        }

        return CreateTemporaryAudioSource();
    }
}
