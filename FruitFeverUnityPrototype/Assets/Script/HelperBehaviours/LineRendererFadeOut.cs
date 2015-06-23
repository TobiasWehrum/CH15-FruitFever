using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LineRendererFadeOut : MonoBehaviour
{
    [SerializeField] private float delayUntilStart;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private bool destroyOnFadeOut = true;

    private float countdown;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        countdown = fadeOutTime;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (countdown <= 0)
            return;

        if (delayUntilStart > 0)
        {
            delayUntilStart -= Time.smoothDeltaTime;
            return;
        }

        countdown -= Time.smoothDeltaTime;

        if (countdown <= 0)
        {
            if (destroyOnFadeOut)
            {
                Destroy(gameObject);
            }
            return;
        }

        var color = lineRenderer.material.GetColor("_TintColor");
        color.a = countdown / fadeOutTime;
        lineRenderer.material.SetColor("_TintColor", color);
    }
}

