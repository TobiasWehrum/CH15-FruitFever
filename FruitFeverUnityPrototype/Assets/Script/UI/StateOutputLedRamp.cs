using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StateOutputLedRamp : StateOutput
{
    [SerializeField] private Image[] images;

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

        var amount = 1 + Mathf.FloorToInt((images.Length - 1) * value);
        for (var i = 0; i < images.Length; i++)
        {
            images[i].enabled = i < amount;
        }

        initialized = true;
    }
}
