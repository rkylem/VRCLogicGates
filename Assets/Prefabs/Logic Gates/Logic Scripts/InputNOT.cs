using UdonSharp;
using UnityEngine;

public class InputNOT : UdonSharpBehaviour
{
    public NotGate notGate;
    public float timeDelayToUpdate = 0.3f;
    float countDownTimer;
    bool startedTimer = false;
    bool inputSignal = false;
    bool inUse = false;

    void Start()
    {
        countDownTimer = timeDelayToUpdate;
    }
    private void Update()
    {
        // update delay
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
    // start timer to send the update in input is in use
    public void UpdateGate()
    {
        if (inUse)
        {
            startedTimer = true;
        }        
    }
    // This function will force the gate to update if the input is in use or not.
    // useful for when you removed the connection but still need to send update
    public void ForceUpdateGate()
    {
        if (inputSignal)
        {// if Input on, Not gate is off
            notGate.NetworkedOnFalse();
        }
        else
        {
            notGate.NetworkedOnTrue();
        }
    }
    void SendUpdate()
    {
        // if still in use after the timer
        if (inUse)
        {
            if (inputSignal)
            {// if Input on, Not gate is off
                notGate.NetworkedOnFalse();
            }
            else
            {
                notGate.NetworkedOnTrue();
            }
        }
    }

    public bool GetInputSignal()
    {
        return inputSignal;
    }
    public void SetInputSignal(bool inSignal)
    {
        inputSignal = inSignal;
    }
    public bool GetInUse()
    {
        return inUse;
    }
    public void SetInUse(bool _inUse)
    {
        inUse = _inUse;
    }
}