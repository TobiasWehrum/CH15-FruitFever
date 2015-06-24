using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SfxManager : SfxManagerBase<SfxManager>
{
    [SerializeField] private AudioClipWithVolume[] foodTaken;
    [SerializeField] private AudioClipWithVolume[] foodEaten;
    [SerializeField] private AudioClipWithVolume gameOver;
    [SerializeField] private AudioClipWithVolume checkSide;
    [SerializeField] private float pan = 0.8f;
    
    public void PlayFoodTaken(Player player)
    {
        PlaySound(foodTaken, PanByPlayer(player));
    }

    public void PlayFoodEaten(Player player)
    {
        PlaySound(foodEaten, PanByPlayer(player));
    }

    public void PlayGameOver()
    {
        PlaySound(gameOver, 0f);
    }

    public void PlayCheckSide()
    {
        PlaySound(checkSide, -1f);
    }

    private float PanByPlayer(Player player)
    {
        return (player.Index == 0) ? -pan : pan;
    }
}
