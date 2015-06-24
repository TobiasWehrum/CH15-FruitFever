using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StateOutputSlider : StateOutput
{
    private bool initialized;
    private float targetValue;
    private GameManager gameManager;
    private Slider slider;
    private Image colorArea;

    public override void SetValue(float value)
    {
        if (!initialized)
        {
            gameManager = GameManager.Instance;
            slider = GetComponentInChildren<Slider>();
            colorArea = slider.fillRect.GetComponentInChildren<Image>();

            slider.value = value;
            colorArea.color = gameManager.StateColorDisplayRange.Evaluate(slider.value);
        }

        targetValue = value;
        initialized = true;
    }

    private void Update()
    {
        if (gameManager.GameOver)
            return;

        Refresh();
    }

    private void Refresh()
    {
        if (!initialized)
            return;

        slider.value = Mathf.MoveTowards(slider.value, targetValue, gameManager.DisplaySliderSpeed);
        colorArea.color = gameManager.StateColorDisplayRange.Evaluate(slider.value);
    }
}
