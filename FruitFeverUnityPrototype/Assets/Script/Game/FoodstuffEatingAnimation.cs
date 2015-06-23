using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FoodstuffEatingAnimation : MonoBehaviourBase
{
    private Player player;
    private int foodstuffIndex;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private RectTransform rectTransform;

    private GameManager gameManager;
    private Countdown countdown;

    public void Initialize(Player player, int foodstuffIndex, Vector3 targetPosition)
    {
        this.player = player;
        this.foodstuffIndex = foodstuffIndex;

        rectTransform = GetComponent<RectTransform>();

        gameManager = GameManager.Instance;
        countdown = new Countdown(gameManager.FruitEatingTime);

        startPosition = rectTransform.position;
        this.targetPosition = targetPosition;
    }

    public void Update()
    {
        if (gameManager.GameOver)
            return;

        if (countdown.Update())
        {
            gameManager.ApplyValues(player, foodstuffIndex);
            Destroy(gameObject);
        }
        else
        {
            rectTransform.position = Vector3.Lerp(startPosition, targetPosition, countdown.PercentEased(gameManager.FruitEatingEase));
        }
    }
}
