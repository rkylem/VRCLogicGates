using UdonSharp;
using UnityEngine;

public class InputSplitter : UdonSharpBehaviour
{
    public LineSplitter lineSplitter;

    public float timeDelayToUpdate = 0.2f;
    float countDownTimer;
    bool startedTimer = false;

    public bool input = false;
    public bool inUse = false;

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
        if (inUse)
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
        if (input)
        {// if input is on, output is on
            //lineSplitter.OnTrue();
            lineSplitter.NetworkedOnTrue();
        }
        else
        {
            //lineSplitter.OnFalse();
            lineSplitter.NetworkedOnFalse();
        }
    }
    void SendUpdate()
    {
        // if still in use after the timer
        if (inUse && input)
        {// little bit simpler
            lineSplitter.NetworkedOnTrue();
        }
        else
        {
            lineSplitter.NetworkedOnFalse();
        }
    }
    public void ResetInput()
    {
        startedTimer = false;
        countDownTimer = timeDelayToUpdate;
        input = false;
        inUse = false;
    }
}
