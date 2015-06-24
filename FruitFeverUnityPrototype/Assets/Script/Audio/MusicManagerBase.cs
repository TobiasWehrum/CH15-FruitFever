using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class MusicManagerBase<T> : PersistentSingletonMonoBehaviour<T> where T : MusicManagerBase<T>
{
    [Serializable]
    protected class Playlist
    {
        [SerializeField] private string title;
        [SerializeField] private AudioClip[] audioClips;
        [SerializeField] private bool randomOrder;

        private int currentIndex = -1;

        public AudioClip GetNextSong(bool reset = false)
        {
            if (audioClips.Length == 0)
                return null;

            if (randomOrder)
            {
                currentIndex = GetRandomIndexExcept(currentIndex);
            }
            else
            {
                if (reset)
                {
                    currentIndex = 0;
                }
                else
                {
                    currentIndex = (currentIndex + 1) % audioClips.Length;
                }
            }

            return audioClips[currentIndex];
        }

        private int GetRandomIndexExcept(int oldIndex)
        {
            if (audioClips.Length <= 1)
                return oldIndex;

            int newIndex;
            do
            {
                newIndex = Random.Range(0, audioClips.Length);
            } while (newIndex == oldIndex);

            return newIndex;
        }

        public bool TitleMatches(string searchedTitle)
        {
            return searchedTitle.ToLowerInvariant().Equals(title.ToLowerInvariant());
        }
    }

    private AudioSource audioSource;

    private AudioClip nextSong;
    private float currentFadingOutTime;
    private float fadingOutTimeLeft;
    private float _volume;
    private Playlist currentPlaylist;

    private Settings settings;

    protected override void OnFirstLoad()
    {
    }

    protected override void Start()
    {
        base.Start();

        if (settings == null)
        {
            settings = Settings.Instance;

            audioSource = GetComponent<AudioSource>();
            audioSource.mute = !settings.Music;
            audioSource.Play();
            
            Volume = 1;
        }
    }

    void Update()
    {
        audioSource.mute = !settings.Music;

        // Fade the current song out and switch to the next song
        if (nextSong != null)
        {
            fadingOutTimeLeft -= Time.deltaTime;
            float volumePercent = FadingVolumePercent;

            if (volumePercent == 0)
            {
                audioSource.volume = Volume;
                audioSource.clip = nextSong;
                audioSource.Play();

                nextSong = null;
            }
            else
            {
                audioSource.volume = Volume * volumePercent;
            }

            return;
        }

        // If there is a playlist and there is no song playing, switch to the next song
        if ((!audioSource.isPlaying) && (currentPlaylist != null))
        {
            audioSource.clip = currentPlaylist.GetNextSong();
            audioSource.Play();
        }
    }

    protected void SwitchToPlaylist(IEnumerable<Playlist> playlists, string playlistTitle, float fadingOutTime)
    {
        var matchingPlaylist = playlists.FirstOrDefault(playlist => playlist.TitleMatches(playlistTitle));
        if (matchingPlaylist != null)
        {
            SwitchToPlaylist(matchingPlaylist, fadingOutTime);
        }
    }

    protected void SwitchToPlaylist(Playlist playlist, float fadingOutTime)
    {
        if (currentPlaylist == playlist)
            return;

        var hadAPlaylistBefore = currentPlaylist != null;

        currentPlaylist = playlist;
        SwitchToSong(currentPlaylist.GetNextSong(true), fadingOutTime, false, true, !hadAPlaylistBefore);
    }

    protected void SwitchToSong(AudioClip clip, float fadingOutTime, bool loop, bool partOfPlaylist, bool fadeEvenIfAlreadyPlaying = false)
    {
        if (!partOfPlaylist)
            currentPlaylist = null;

        audioSource.loop = loop;

        if ((audioSource.clip == clip) && !fadeEvenIfAlreadyPlaying && (nextSong == null))
            return;

        if ((audioSource.isPlaying) && (fadingOutTime > 0))
        {
            nextSong = clip;
            fadingOutTimeLeft = fadingOutTime;
            currentFadingOutTime = fadingOutTime;
        }
        else
        {
            audioSource.clip = clip;
            audioSource.Play();
            nextSong = null;
            currentFadingOutTime = 0;
        }
    }

    public float Volume
    {
        get { return _volume; }
        set
        {
            _volume = value;
            audioSource.volume = FadingVolumePercent * _volume;
        }
    }

    private float FadingVolumePercent { get { return (nextSong == null) ? 1 : Mathf.Clamp01(fadingOutTimeLeft / currentFadingOutTime); } }
}
