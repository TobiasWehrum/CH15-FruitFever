using System.Linq;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private int[] values;
    public int Index;
    private string[] buttons;
    private GameManager gameManager;

    public PlayerDisplay Display { get; private set; }

    public bool AllValuesZero
    {
        get { return values.All(value => value == 0); }
    }

    public void Initialize(int index, int[] startValues, PlayerDisplay display)
    {
        this.Index = index;
        this.Display = display;
        values = (int[]) startValues.Clone();

        gameManager = GameManager.Instance;
        buttons = new string[gameManager.FoodstuffCount];
        for (var foodstuffIndex = 0; foodstuffIndex < buttons.Length; foodstuffIndex++)
        {
            buttons[foodstuffIndex] = string.Format("P{0}F{1}", index, foodstuffIndex);
        }
    }

    private void Start()
    {
        RefreshDisplay();
    }

    private void Update()
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            if (Input.GetButtonDown(buttons[i]))
            {
                gameManager.EatFoodstuff(this, i);
            }
        }
    }

    private void RefreshDisplay()
    {
        Display.Refresh(values);
    }

    public void ChangeValues(int[] add)
    {
        for (var i = 0; i < add.Length; i++)
        {
            values[i] = Mathf.Clamp(values[i] + add[i], -gameManager.StepCountEachSide, gameManager.StepCountEachSide);
        }
        RefreshDisplay();
    }
}
