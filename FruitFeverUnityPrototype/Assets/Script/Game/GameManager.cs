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
    [SerializeField] private Canvas canvas;
    [SerializeField] private EffectDisplayRow effectDisplayRowPrefab;
    [SerializeField] private EffectDisplayRow effectDisplayRowEmptyPrefab;
    [SerializeField] private Sprite[] symbolsSliders;
    [SerializeField] private Sprite symbolArrowUp;
    [SerializeField] private Sprite symbolArrowDown;
    [SerializeField] private PlayerDisplay goalDisplay;
    [SerializeField] private GameObject letters;
    [SerializeField] private RectTransform[] panels;

    public Gradient StateColorDisplayRange { get { return stateColorDisplayRange; } }
    public int FoodstuffCount { get { return foodstuffs.Length; } }
    public int StepCountEachSide { get { return settings.Amplitude; } }
    public float FruitEatingTime { get { return fruitEatingTime; } }
    public EaseType FruitEatingEase { get { return fruitEatingEase; } }
    public float DisplaySliderSpeed { get { return displaySliderSpeed; } }

    private int[][] ruleset;
    private Settings settings;
    private SfxManager sfxManager;

    private Transform currentDisplay;

    public bool GameOver { get; private set; }
    public Player[] Players { get; private set; }

    private void Awake()
    {
        sfxManager = SfxManager.Instance;
        settings = Settings.Instance;

        Random.seed = settings.Seed;

        stateCount = settings.Organs;

        difficultyDisplay.text = String.Format(difficultyDisplay.text, settings.Amplitude);

        pressToRestart.SetActive(false);

        for (var i = (settings.Organs + 1); i < foodstuffs.Length; i++)
        {
            foodstuffs[i].gameObject.SetActive(false);
        }

        foodstuffs = foodstuffs.Where(foodstuff => foodstuff.gameObject.activeSelf).ToArray();

        UseRulesetTemplate(rulesetTemplates[settings.Organs - 1]);

        var startValues = new int[stateCount];
        do
        {
            for (var i = 0; i < startValues.Length; i++)
            {
                startValues[i] = Random.Range(1, StepCountEachSide) * MathUtil.RandomSign;
                if (i >= settings.Organs)
                {
                    startValues[i] = 0;
                }
            }
        }
        while (startValues.All(value => value == 0));

        Players = new Player[playerDisplays.Length];
        for (var i = 0; i < playerDisplays.Length; i++)
        {
            var player = UnityHelper.InstantiatePrefab(playerPrefab);
            player.Initialize(i, startValues, playerDisplays[i]);

            Players[i] = player;
        }

        if (settings.Transparency > 0)
        {
            var showTransparency = new bool[foodstuffs.Length];
            for (var i = 0; i < Mathf.Min(settings.Transparency, foodstuffs.Length); i++)
            {
                showTransparency[i] = true;
            }

            showTransparency.Shuffle();

            for (var i = 0; i < foodstuffs.Length; i++)
            {
                DisplayEffect(panels[i], ruleset[i], showTransparency[i]);
            }
        }
    }

    private void Start()
    {
        if (goalDisplay != null)
        {
            goalDisplay.Refresh(new int[stateCount]);
        }

        if (Players[0].ArduinoInput.IsConnected)
        {
            letters.SetActive(false);
        }
    }

    private void OnEnable()
    {
        settings.EventAmplitudeChanged += Restart;
        settings.EventTransparencyChanged += Restart;
        settings.EventOrgansChanged += Restart;
    }

    private void OnDisable()
    {
        settings.EventAmplitudeChanged -= Restart;
        settings.EventTransparencyChanged -= Restart;
        settings.EventOrgansChanged -= Restart;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
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

        if (!foodstuffs[foodstuffIndex].gameObject.activeSelf)
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

    private void Restart()
    {
        if (settings.Randomized)
        {
            Settings.Instance.Seed = Random.Range(int.MinValue, int.MaxValue);
        }

        Application.LoadLevel(Application.loadedLevel);
    }

    private void RestartNoSeeding()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    private void DisplayEffect(RectTransform panel, int[] rules, bool show)
    {
        //var xPosition = rectTransform.anchoredPosition.x + rectTransform.rect.center.x;
        //var yPosition = rectTransform.anchoredPosition.y + rectTransform.rect.yMin;

        var xPosition = panel.anchoredPosition.x;
        var yPosition = panel.anchoredPosition.y;

        if (show)
        {
            var count = rules.Where((t, i) => i < settings.Organs).Count(value => value != 0);
            yPosition += effectDisplayRowPrefab.GetComponent<RectTransform>().rect.height * count / 2f;

            for (var i = 0; i < rules.Length; i++)
            {
                if (i >= settings.Organs)
                    continue;

                var value = rules[i];
                if (value == 0)
                    continue;

                var display = UnityHelper.InstantiatePrefab(effectDisplayRowPrefab);
                var displayRectTransform = display.GetComponent<RectTransform>();
                displayRectTransform.SetParent(panel.parent.transform);
                displayRectTransform.localScale = Vector3.one;
                displayRectTransform.anchoredPosition = new Vector2(xPosition, yPosition);

                yPosition -= displayRectTransform.rect.height;

                display.ArrowSprite = (value == 1) ? symbolArrowUp : symbolArrowDown;
                display.SymbolSprite = symbolsSliders[i];
            }
        }
        else
        {
            var display = UnityHelper.InstantiatePrefab(effectDisplayRowEmptyPrefab);
            var displayRectTransform = display.GetComponent<RectTransform>();
            displayRectTransform.SetParent(panel.parent.transform);
            displayRectTransform.localScale = Vector3.one;
            displayRectTransform.anchoredPosition = new Vector2(xPosition, yPosition);
        }
    }

    public void SetPickedUpFruit(int fruitNumber)
    {
        if (currentDisplay != null)
        {
            Destroy(currentDisplay.gameObject);
            currentDisplay = null;
        }

        if (fruitNumber == 0)
            return;

        currentDisplay = Instantiate(foodstuffs[fruitNumber - 1]);
        var rectTransform = currentDisplay.GetComponent<RectTransform>();
        rectTransform.SetParent(canvas.transform);
        rectTransform.anchoredPosition = new Vector2(0, 100f);
    }
}
