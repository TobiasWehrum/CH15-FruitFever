using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviourBase
{
    [SerializeField] private Slider[] sliders;
    [SerializeField] private GameObject playerWonDisplay;

    private GameManager gameManager;
    private Image[] colorAreas;
    private float[] targetValues;
    private bool initialized;

    public Vector3 FoodstuffTargetPosition
    {
        get { return GetComponent<RectTransform>().position; }
    }

    private void Awake()
    {
        gameManager = GameManager.Instance;

        colorAreas = new Image[sliders.Length];
        for (var i = 0; i < sliders.Length; i++)
        {
            colorAreas[i] = sliders[i].fillRect.GetComponentInChildren<Image>();
        }

        targetValues = new float[sliders.Length];

        playerWonDisplay.SetActive(false);
    }

    public void Refresh(int[] values)
    {
        for (var i = 0; i < sliders.Length; i++)
        {
            var value = Mathf.InverseLerp(-gameManager.StepCountEachSide, gameManager.StepCountEachSide, values[i]);
            targetValues[i] = value;

            if (!initialized)
            {
                sliders[i].value = targetValues[i];
                colorAreas[i].color = gameManager.StateColorDisplayRange.Evaluate(value);
            }
        }
        initialized = true;
    }

    public void Update()
    {
        for (var i = 0; i < sliders.Length; i++)
        {
            sliders[i].value = Mathf.MoveTowards(sliders[i].value, targetValues[i], gameManager.DisplaySliderSpeed);
            colorAreas[i].color = gameManager.StateColorDisplayRange.Evaluate(sliders[i].value);
        }
    }

    public void Won()
    {
        playerWonDisplay.SetActive(true);
    }
}
