﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StateOutputLedRamp : StateOutput
{
    [SerializeField] private Image[] images;
    [SerializeField] private bool full;
    [SerializeField] private bool showOthersTransparently;
    [SerializeField] private Color transparentColor;

    private bool initialized;
    private GameManager gameManager;

    public override void SetValue(float value)
    {
        if (!initialized)
        {
            gameManager = GameManager.Instance;
        }

        var color = gameManager.StateColorDisplayRange.Evaluate(value);
        foreach (var image in images)
        {
            image.color = color;
        }

        var maxValue = images.Length;;
        if (!full)
        {
            maxValue = Mathf.Min(Settings.Instance.Amplitude * 2 + 1, maxValue);
        }

        var amount = 1 + Mathf.FloorToInt((maxValue - 1) * value);
        for (var i = 0; i < images.Length; i++)
        {
            images[i].enabled = i < amount;
            if (!images[i].enabled && (i < maxValue) && showOthersTransparently)
            {
                images[i].enabled = true;
                images[i].color = transparentColor;
            }
        }

        initialized = true;
    }
}
