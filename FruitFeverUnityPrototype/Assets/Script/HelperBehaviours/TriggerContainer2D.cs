using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TriggerContainer2D : MonoBehaviour
{
    public delegate void TriggerAction2D(TriggerContainer2D triggerContainer, Collider2D other);

    public event TriggerAction2D EventTriggerEnter;
    public event TriggerAction2D EventTriggerStay;
    public event TriggerAction2D EventTriggerExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (EventTriggerEnter != null)
            EventTriggerEnter(this, other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (EventTriggerStay != null)
            EventTriggerStay(this, other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (EventTriggerExit != null)
            EventTriggerExit(this, other);
    }
}