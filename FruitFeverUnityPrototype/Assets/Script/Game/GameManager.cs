using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private PlayerDisplay[] playerDisplays;
    [SerializeField] private int stateCount = 3;
    [SerializeField] private Gradient stateColorDisplayRange;
    [SerializeField] private string[] rulesetTemplates;
    [SerializeField] private BetweenInt rulesetVariation = new BetweenInt(1, 3);
    [SerializeField] private Transform[] foodstuffs;
    [SerializeField] private float fruitEatingTime = 1f;
    [SerializeField] private EaseType fruitEatingEase = EaseType.Linear;
    [SerializeField] private float displaySliderSpeed = 1f;
    [SerializeField] private GameObject pressToRestart;
    [SerializeField] private bool debugPrintRuleset = false;
    [SerializeField] private Text difficultyDisplay;

    public Gradient StateColorDisplayRange { get { return stateColorDisplayRange; } }
    public int FoodstuffCount { get { return foodstuffs.Length; } }
    public int StepCountEachSide { get { return settings.Difficulty; } }
    public float FruitEatingTime { get { return fruitEatingTime; } }
    public EaseType FruitEatingEase { get { return fruitEatingEase; } }
    public float DisplaySliderSpeed { get { return displaySliderSpeed; } }

    private int[][] ruleset;
    private Settings settings;
    private SfxManager sfxManager;

    public bool GameOver { get; private set; }

    private void Awake()
    {
        sfxManager = SfxManager.Instance;
        settings = Settings.Instance;

        Debug.Log(settings);
        difficultyDisplay.text = String.Format(difficultyDisplay.text, settings.Difficulty);

        pressToRestart.SetActive(false);

        var startValues = new int[stateCount];
        do
        {
            for (var i = 0; i < startValues.Length; i++)
            {
                startValues[i] = Random.Range(-StepCountEachSide, StepCountEachSide);
            }
        }
        while (startValues.All(value => value == 0));

        for (var i = 0; i < playerDisplays.Length; i++)
        {
            var player = UnityHelper.InstantiatePrefab(playerPrefab);
            player.Initialize(i, startValues, playerDisplays[i]);
        }

        UseRulesetTemplate(rulesetTemplates.RandomElement());
    }

    private void OnEnable()
    {
        settings.EventDifficultyChanged += OnDifficultyChanged;
    }

    private void OnDisable()
    {
        settings.EventDifficultyChanged -= OnDifficultyChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            sfxManager.PlayCheckSide();
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
            if ((Random.value < 0.5) || (rulesetVariation.Range <= 1))
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

        if (debugPrintRuleset)
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

        var originalFoodstuff = foodstuffs[foodstuffIndex];
        var targetPosition = playerDisplays[player.Index].FoodstuffTargetPosition;
        var fruitCopy = UnityHelper.InstantiatePrefab(originalFoodstuff);
        fruitCopy.transform.SetParent(originalFoodstuff.parent);
        var fruitCopyRect = fruitCopy.gameObject.GetComponent<RectTransform>();
        var originalRect = originalFoodstuff.gameObject.GetComponent<RectTransform>();
        fruitCopyRect.position = originalRect.position;
        fruitCopy.localScale = originalRect.localScale;
        var eatingAnimation = fruitCopy.gameObject.AddComponent<FoodstuffEatingAnimation>();
        eatingAnimation.Initialize(player, foodstuffIndex, targetPosition);

        sfxManager.PlayFoodTaken(player);
    }

    public void ApplyValues(Player player, int foodstuffIndex)
    {
        sfxManager.PlayFoodEaten(player);

        player.ChangeValues(ruleset[foodstuffIndex]);

        player.Display.FruitEaten();

        if (player.AllValuesZero)
        {
            SetGameOver(player);
        }
    }

    private void SetGameOver(Player player)
    {
        if (GameOver)
            return;

        player.Display.Won();
        GameOver = true;

        pressToRestart.SetActive(true);

        sfxManager.PlayGameOver();
    }

    private void OnDifficultyChanged(int newDifficulty)
    {
        Restart();
    }

    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
