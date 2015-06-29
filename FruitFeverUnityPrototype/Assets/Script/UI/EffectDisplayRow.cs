using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EffectDisplayRow : MonoBehaviourBase
{
    [SerializeField] private Image symbol;
    [SerializeField] private Image arrow;

    public Sprite SymbolSprite
    {
        get { return symbol.sprite; }
        set { symbol.sprite = value; }
    }

    public Sprite ArrowSprite
    {
        get { return arrow.sprite; }
        set { arrow.sprite = value; }
    }
}
