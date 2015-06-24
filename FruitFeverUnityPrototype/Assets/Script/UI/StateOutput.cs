using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public abstract class StateOutput : MonoBehaviourBase
{
    public abstract void SetValue(float value);
}
