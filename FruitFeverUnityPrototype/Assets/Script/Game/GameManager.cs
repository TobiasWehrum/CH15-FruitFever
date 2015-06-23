using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private PlayerDisplay[] playerDisplays;
    [SerializeField] private int stateCount = 3;
    [SerializeField] private Gradient stateColorDisplayRange;
    [SerializeField] private string[] rulesetTemplates;
    [SerializeField] private int stepCountEachSide = 4;
    [SerializeField] private BetweenInt rulesetVariation = new BetweenInt(1, 3);
    [SerializeField] private Transform[] foodstuffs;

    public Gradient StateColorDisplayRange { get { return stateColorDisplayRange; } }
    public int FoodstuffCount { get { return foodstuffs.Length; } }
    public int StepCountEachSide { get { return stepCountEachSide; } }

    private int[][] ruleset;

    public bool GameOver { get; private set; }

    private void Awake()
    {
        var startValues = new int[stateCount];
        for (var i = 0; i < startValues.Length; i++)
        {
            startValues[i] = Random.Range(-stepCountEachSide, stepCountEachSide);
        }

        for (var i = 0; i < playerDisplays.Length; i++)
        {
            var player = UnityHelper.InstantiatePrefab(playerPrefab);
            player.Initialize(i, startValues, playerDisplays[i]);
        }

        UseRulesetTemplate(rulesetTemplates.RandomElement());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }

    private void UseRulesetTemplate(string rulesetTemplate)
    {
        var ruleParts = rulesetTemplate.Split('|');
        ruleParts.Shuffle();

        var stateCharacters = new char[stateCount];
        var availableValues = new List<int>[stateCount];
        for (var i = 0; i < stateCount; i++)
        {
            stateCharacters[i] = (char)(65 + i);

            var valuesForState = new List<int>();
            availableValues[i] = valuesForState;

            var sign = MathUtil.RandomSign;
            if (Random.value < 0.5)
            {
                valuesForState.Add(1 * sign);
                valuesForState.Add(rulesetVariation.RandomInclusive * -sign);
            }
            else
            {
                var baseValue = rulesetVariation.RandomExclusive;
                valuesForState.Add(baseValue * sign);
                valuesForState.Add((baseValue + 1) * -sign);
            }
        }
        stateCharacters.Shuffle();

        ruleset = new int[ruleParts.Length][];

        for (var rulePartIndex = 0; rulePartIndex < ruleParts.Length; rulePartIndex++)
        {
            var rulePart = ruleParts[rulePartIndex];
            ruleset[rulePartIndex] = new int[stateCount];
            for (var stateIndex = 0; stateIndex < stateCount; stateIndex++)
            {
                var character = stateCharacters[stateIndex];
                if (!rulePart.Contains(character))
                    continue;

                ruleset[rulePartIndex][stateIndex] = availableValues[stateIndex].RandomElement(true);
            }
        }

        ruleset.Shuffle();

        DebugPrintRuleset();
    }

    private void DebugPrintRuleset()
    {
        var result = "Ruleset:\n";
        for (var ruleIndex = 0; ruleIndex < ruleset.Length; ruleIndex++)
        {
            result += "- " + ruleset[ruleIndex].ToOneLineString(", ", "") + "\n";
        }
        Debug.Log(result);
    }

    public void EatFoodstuff(Player player, int foodstuffIndex)
    {
        if (GameOver)
            return;

        player.ChangeValues(ruleset[foodstuffIndex]);

        if (player.AllValuesZero)
        {
            SetGameOver(player);
        }
    }

    private void SetGameOver(Player player)
    {
        playerDisplays[player.Index].Won();
        GameOver = true;
    }
}
