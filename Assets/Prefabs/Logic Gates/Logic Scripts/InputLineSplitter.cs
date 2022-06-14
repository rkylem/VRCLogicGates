using UdonSharp;
using UnityEngine;

public class InputLineSplitter : UdonSharpBehaviour
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
        if (input)
        {// if input is on, output is on
            lineSplitter.OnTrue();
            //orGate.NetworkedOnTrue();
        }
        else
        {
            lineSplitter.OnFalse();
            //orGate.NetworkedOnFalse();
        }
    }
    void SendUpdate()
    {
        // if still in use after the timer
        if (inUse && input)
        {// little bit simpler
            lineSplitter.OnTrue();
        }
        else
        {
            lineSplitter.OnFalse();
        }
    }
}
