﻿using UdonSharp;
using UnityEngine;

public class LocalInputNot : UdonSharpBehaviour
{
    public LocalNotGate notGate;
    public float timeDelayToUpdate = 0.3f;
    float countDownTimer;
    bool startedTimer = false;

    public bool inputSignal = false;
    public bool inUse = false;

    void Start()
    {
        countDownTimer = timeDelayToUpdate;
    }
    private void Update()
    {
        if (startedTimer)
        {
            countDownTimer -= Time.deltaTime;
            if (countDownTimer <= 0)
            {
                countDownTimer = timeDelayToUpdate;
                startedTimer = false;
                SendUpdate();
            }
        }
    }
    public void UpdateGate()
    {
        if (inUse)
        {
            startedTimer = true;
        }
    }
    public void ForceUpdateGate()
    {
        if (inputSignal)
        {
            notGate.OnFalse();
        }
        else
        {
            notGate.OnTrue();
        }
    }
    void SendUpdate()
    {
        if (inUse)
        {
            if (inputSignal)
            {
                notGate.OnFalse();
            }
            else
            {
                notGate.OnTrue();
            }
        }
    }
}