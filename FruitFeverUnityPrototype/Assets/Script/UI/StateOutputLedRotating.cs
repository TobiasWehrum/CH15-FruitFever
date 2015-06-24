using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StateOutputLedRotating : StateOutput
{
    [SerializeField] private Image[] images;
    [SerializeField] private BetweenFloat nextLedTime;
    [SerializeField] private float fadeOutAmount = 0.1f;

    private bool initialized;
    private float passedTime;
    private float currentNextImageTime;
    private GameManager gameManager;
    private int currentIndex;

    public override void SetValue(float value)
    {
        if (!initialized)
        {
            gameManager = GameManager.Instance;

            images = GetComponentsInChildren<Image>();
            foreach (var image in images)
            {
                //image.enabled = false;
                image.color = new Color(0, 0, 0, 0);
            }

            images[currentIndex].color = images[currentIndex].color.ChangeAlpha(1f);
        }

        var color = gameManager.StateColorDisplayRange.Evaluate(value);
        foreach (var image in images)
        {
            image.color = color.ChangeAlpha(image.color.a);
        }

        currentNextImageTime = nextLedTime.Lerp(value);
        initialized = true;
    }

    private void Update()
    {
        passedTime += Time.deltaTime;
        if (passedTime >= currentNextImageTime)
        {
            passedTime -= currentNextImageTime;

            //images[currentIndex].enabled = false;

            currentIndex = (currentIndex + 1) % images.Length;

            foreach (var image in images)
            {
                image.color = image.color.ChangeAlpha(image.color.a - fadeOutAmount);
            }

            //images[currentIndex].enabled = true;
            images[currentIndex].color = images[currentIndex].color.ChangeAlpha(1f);
        }
    }
}
