using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviourBase
{
    [SerializeField] private StateOutput[] outputs;
    [SerializeField] private GameObject playerWonDisplay;
    [SerializeField] private Text fruitsEatenDisplay;
    [SerializeField] private GameObject signalObject;

    private GameManager gameManager;
    private int fruitsEaten;
    private string fruitsEatenFormatString;

    public Vector3 FoodstuffTargetPosition
    {
        get { return GetComponent<RectTransform>().position; }
    }

    public bool Signal
    {
        get { return signalObject.activeSelf; }
        set { signalObject.SetActive(value); }
    }

    private void Awake()
    {
        gameManager = GameManager.Instance;
        playerWonDisplay.SetActive(false);

        for (var i = 0; i < outputs.Length; i++)
        {
            outputs[i].gameObject.SetActive(i < Settings.Instance.Organs);
        }

        outputs = outputs.Where(output => output.gameObject.activeSelf).ToArray();

        fruitsEatenFormatString = fruitsEatenDisplay.text;
        RefreshFruitsEatenDisplay();

        Signal = false;
    }

    public void Refresh(int[] values)
    {
        for (var i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] == null)
                continue;

            var value = Mathf.InverseLerp(-gameManager.StepCountEachSide, gameManager.StepCountEachSide, values[i]);
            outputs[i].SetValue(value);
        }
    }

    public void Won()
    {
        playerWonDisplay.SetActive(true);

        Debug.Log("Winning player fruits eaten: " + fruitsEaten);
    }

    public void FruitEaten()
    {
        fruitsEaten++;
        RefreshFruitsEatenDisplay();
    }

    private void RefreshFruitsEatenDisplay()
    {
        fruitsEatenDisplay.text = String.Format(fruitsEatenFormatString, fruitsEaten);
    }

    
}
