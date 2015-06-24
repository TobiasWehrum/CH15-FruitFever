using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StateOutputLedBeating : StateOutput
{
    [SerializeField] private BetweenFloat blinkingTime;

    private Image[] images;

    private bool initialized;
    private float passedTime;
    private float currentBlinkingTime;
    private GameManager gameManager;

    public override void SetValue(float value)
    {
        if (!initialized)
        {
            gameManager = GameManager.Instance;

            images = GetComponentsInChildren<Image>();
        }

        var color = gameManager.StateColorDisplayRange.Evaluate(value);
        foreach (var image in images)
        {
            image.color = color;
        }

        currentBlinkingTime = blinkingTime.Lerp(value);
        initialized = true;
    }

    private void Update()
    {
        passedTime += Time.deltaTime;
        if (passedTime >= currentBlinkingTime)
        {
            passedTime -= currentBlinkingTime;
            foreach (var image in images)
            {
                image.enabled = !image.enabled;
            }
        }
    }
}
