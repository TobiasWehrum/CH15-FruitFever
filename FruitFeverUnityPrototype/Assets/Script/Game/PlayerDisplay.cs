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

    private void Awake()
    {
        gameManager = GameManager.Instance;

        colorAreas = new Image[sliders.Length];
        for (var i = 0; i < sliders.Length; i++)
        {
            colorAreas[i] = sliders[i].fillRect.GetComponentInChildren<Image>();
        }

        playerWonDisplay.SetActive(false);
    }

    public void Refresh(int[] values)
    {
        for (var i = 0; i < sliders.Length; i++)
        {
            var value = Mathf.InverseLerp(-gameManager.StepCountEachSide, gameManager.StepCountEachSide, values[i]);
            sliders[i].value = value;
            colorAreas[i].color = gameManager.StateColorDisplayRange.Evaluate(value);
        }
    }

    public void Won()
    {
        playerWonDisplay.SetActive(true);
    }
}
