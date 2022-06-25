using UdonSharp;
using UnityEngine;

public class InputsOR : UdonSharpBehaviour
{
    public ORGate orGate;

    public float timeDelayToUpdate = 0.2f;
    float countDownTimer;
    bool startedTimer = false;

    public bool inputA = false;
    public bool inputB = false;
    public bool aInUse = false;
    public bool bInUse = false;

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
                startedTimer = false;
                countDownTimer = timeDelayToUpdate;
                SendUpdate();
            }
        }
    }
    // start timer to send the update in input is in use
    public void UpdateGate()
    {
        if (aInUse || bInUse)
        {
            startedTimer = true;
        }
    }
    // This function will force the gate to update if the input is in use or not.
    // useful for when you removed the connection but still need to send update
    public void ForceUpdateGate()
    {
        startedTimer = false;
        countDownTimer = timeDelayToUpdate;
        if (inputA || inputB)
        {// if either input is on, output is on
            //orGate.OnTrue();
            orGate.NetworkedOnTrue();
        }
        else
        {
            //orGate.OnFalse();
            orGate.NetworkedOnFalse();
        }
    }
    void SendUpdate()
    {
        // if still in use after the timer
        if (aInUse || bInUse)
        {
            if (inputA || inputB)
            {// if either input is on, output is on
                orGate.NetworkedOnTrue();
                //orGate.OnTrue();
            }
            else
            {
                //orGate.OnFalse();
                orGate.NetworkedOnFalse();
            }
        }
    }

    public void ResetInputs()
    {
        startedTimer = false;
        countDownTimer = timeDelayToUpdate;
        aInUse = false;
        bInUse = false;
        inputA = false;
        inputB = false;
    }    
}