﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

#if !UNITY_WEBPLAYER
using System.IO.Ports;
#endif

public class ArduinoInput : MonoBehaviourBase
{
    [SerializeField] private int comNumber = 2;
    [SerializeField] private int baudRate = 9600;

    private bool activated;

    public bool Activated
    {
        get
        {
            lock (this)
            {
                return activated;
            }
        }
    }

#if !UNITY_WEBPLAYER
    private SerialPort stream;
    private Thread thread;

    private void Awake()
    {
        //Debug.Log(SerialPort.GetPortNames().ToOneLineString());
        stream = new SerialPort("COM" + comNumber, baudRate);
        thread = new Thread(ReadDataThread);
    }

    private void OnEnable()
    {
        try
        {
            stream.Open();
            if (stream.IsOpen)
            {
                thread.Start();
            }
        }
        catch (IOException exception)
        {
            Debug.LogWarning("Error while connecting to the Arduino: " + exception.Message);
        }
    }

    private void OnDisable()
    {
        stream.Close();
        thread.Abort();
    }

    private void ReadDataThread()
    {
        while (true)
        {
            lock (this)
            {
                var line = stream.ReadLine();
                activated = line.Equals("1");
            }
        }
    }
#endif
}