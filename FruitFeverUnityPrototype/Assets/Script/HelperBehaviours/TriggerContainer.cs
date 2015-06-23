using System;
using UnityEngine;
using System.Collections;

public class TriggerContainer : MonoBehaviour
{
    public delegate void TriggerAction(TriggerContainer triggerContainer, Collider other);

    public event TriggerAction EventTriggerEnter;
    public event TriggerAction EventTriggerStay;
    public event TriggerAction EventTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (EventTriggerEnter != null)
            EventTriggerEnter(this, other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (EventTriggerStay != null)
            EventTriggerStay(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (EventTriggerExit != null)
            EventTriggerExit(this, other);
    }
}

