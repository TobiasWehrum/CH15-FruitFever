using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GradientDisplay : MonoBehaviourBase
{
    //[SerializeField] private int textureWidth = 512;
    //[SerializeField] private int textureHeight = 512;
    //[SerializeField] private BetweenFloat cutoffValues = new BetweenFloat(0.1f, 0.9f);

    private Texture2D texture;
    private RawImage imageDisplay;

    private void Awake()
    {
        var rectTransform = GetComponent<RectTransform>();
        var textureWidth = (int) rectTransform.rect.width;
        var textureHeight = (int) rectTransform.rect.height;

        var gradient = GameManager.Instance.StateColorDisplayRange;

        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false, false);
        var colors = new Color[textureWidth * textureHeight];
        for (var x = 0; x < textureWidth; x++)
        {
            //var color = gradient.Evaluate(Mathf.Clamp((float) x / (textureWidth - 1), cutoffValues.From, cutoffValues.To));
            var color = gradient.Evaluate((float) x / (textureWidth - 1));
            for (var y = 0; y < textureHeight; y++)
            {
                colors[x + y * textureWidth] = color;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        imageDisplay = GetComponent<RawImage>();
        imageDisplay.texture = texture;
    }
}
