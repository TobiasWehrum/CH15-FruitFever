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
    [SerializeField] private int playerIndex;
    [SerializeField] private bool rtsEnable;
    [SerializeField] private bool dtsEnable;
    [SerializeField] private int readTimeout = 10;
    [SerializeField] private int writeTimeout = 10;

    private int readNumber;
    private bool dataUpdated;

    private int previousNumber;
    private GameManager gameManager;

#if !UNITY_WEBPLAYER
    private SerialPort stream;
    //private Thread thread;

    private bool quit = false;

    private void Awake()
    {
        //Debug.Log(SerialPort.GetPortNames().ToOneLineString());
        stream = new SerialPort("COM" + comNumber, baudRate);
        stream.RtsEnable = rtsEnable;
        stream.DtrEnable = dtsEnable;
        stream.ReadTimeout = readTimeout;
        stream.WriteTimeout = writeTimeout;
        //thread = new Thread(ReadDataThread);

        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        quit = false;
        try
        {
            stream.Open();
            if (stream.IsOpen)
            {
                //thread.Start();
            }
        }
        catch (IOException exception)
        {
            Debug.LogWarning("Error while connecting to the Arduino: " + exception.Message);
        }
    }

    private void OnDisable()
    {
        quit = true;

        /*
        if (stream.IsOpen)
        {
            var data = new byte[4];
            data[0] = 100;
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = 99;
            }
            stream.Write(data, 0, data.Length);
        }
         */

        stream.Close();
        //thread.Abort();
    }

    private void Update()
    {
        try
        {
            var line = stream.ReadLine();
            int newNumber;
            if (int.TryParse(line, out newNumber))
            {
                if (newNumber != previousNumber)
                {
                    NumberChanged(previousNumber, newNumber);
                    previousNumber = newNumber;
                }
            }
        }
        catch (Exception)
        {
        }
        /*
        if (dataUpdated)
        {
            var number = 0;
            lock (this)
            {
                number = readNumber;
                dataUpdated = false;
            }

            if (number != previousNumber)
            {
                NumberChanged(previousNumber, number);
                previousNumber = number;
            }
        }
         */
    }

    private void NumberChanged(int from, int to)
    {
        // Changed to unknown number
        if ((to > 5) || (to < 0))
        {
            Debug.LogError("Changed to undefined number #" + to);
            return;
        }

        // Changed to signal
        if (to == 5)
        {
            Signal(true);
            return;
        }

        // Changed from signal
        if (from == 5)
        {
            Signal(false);
        }

        // Changed from fruit...
        if ((from >= 1) && (from <= 4))
        {
            // ...to no fruit
            if (to == 0)
            {
                FruitEaten(from);
            }
            // ...to another fruit (everything but 0..4 had already returned)
            else
            {
                Debug.LogError(String.Format("Changed from fruit #{0} to #{1}?", from, to));
            }
        }

        // Changed to fruit
        if ((to >= 1) && (to <= 4))
        {
            FruitPickedUp(to);
        }
    }

    private void FruitPickedUp(int fruitNumber)
    {
        Debug.Log(PlayerName + " has picked up fruit #" + fruitNumber);
        gameManager.SetPickedUpFruit(fruitNumber);
    }

    private void FruitEaten(int fruitNumber)
    {
        Debug.Log(PlayerName + " has eaten fruit #" + fruitNumber);

        gameManager.ApplyValues(gameManager.Players[playerIndex], fruitNumber - 1);
        gameManager.SetPickedUpFruit(0);
    }

    private void Signal(bool value)
    {
        Debug.Log(PlayerName + " has sent a signal");

        gameManager.Players[playerIndex].Display.Signal = value;
    }

    /*
    private void ReadDataThread()
    {
        while (!quit)
        {
            Thread.Sleep(100);
            var line = stream.ReadLine();
            if (line.Length <= 0)
                continue;

            int newNumber;
            if (int.TryParse(line, out newNumber))
            {
                lock (this)
                {
                    readNumber = newNumber;
                    dataUpdated = true;
                }
            }

            Thread.Sleep(0);
        }
    }
     */

    private string PlayerName
    {
        get { return (playerIndex == 0) ? "Left Player" : "Right Player"; }
    }

    public int PlayerIndex
    {
        get { return playerIndex; }
    }

#endif

    public void RefreshValues(int[] values)
    {
#if !UNITY_WEBPLAYER
        if (!stream.IsOpen)
            return;

        var data = new byte[4];
        data[0] = 100;
        for (var i = 0; i < values.Length; i++)
        {
            data[i + 1] = (byte)(values[i] + gameManager.StepCountEachSide);
        }
        for (var i = values.Length; i < 3; i++)
        {
            data[i + 1] = 99;
        }
        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
#endif
    }

    public bool IsConnected
    {
        get
        {
#if UNITY_WEBPLAYER
            return false,
#else
            return stream.IsOpen;
#endif
        }
    }
}
