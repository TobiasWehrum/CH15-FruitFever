using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MusicManagerBase<MusicManager>
{
    [SerializeField] private AudioClip menuSong;
    [SerializeField] private Playlist ingamePlaylist;
    [SerializeField] private AudioClip gameOverSong;
    [SerializeField] private float fadingToMenu = 2f;
    [SerializeField] private float fadingToOrBetweenIngamePlaylists = 1f;
    [SerializeField] private float fadingToGameOver = 0.5f;

    public void PlayMenuSong()
    {
        SwitchToSong(menuSong, fadingToMenu, true, false);
    }

    public void PlayGameOver()
    {
        SwitchToSong(gameOverSong, fadingToGameOver, true, false);
    }

    public void PlayIngamePlaylist()
    {
        SwitchToPlaylist(ingamePlaylist, fadingToOrBetweenIngamePlaylists);
    }
}
