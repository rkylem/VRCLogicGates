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
        // dont forget to remove the scripts on the individual input lines
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
        if (aInUse || bInUse)
        {
            startedTimer = true;
        }
    }
    // This function will force the gate to update if the input is in use or not.
    // useful for when you removed the connection but still need to send update
    public void ForceUpdateGate()
    {
        if (inputA || inputB)
        {// if either input is on, output is on
            orGate.NetworkedOnTrue();            
        }
        else
        {
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
            }
            else
            {
                orGate.NetworkedOnFalse();
            }
        }
    }
}