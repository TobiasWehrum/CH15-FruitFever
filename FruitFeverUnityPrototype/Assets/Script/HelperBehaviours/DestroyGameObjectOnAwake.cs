using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DestroyGameObjectOnAwake : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Awake()
    {
        DestroyImmediate(target);
    }
}

