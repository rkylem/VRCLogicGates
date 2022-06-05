using UdonSharp;
using UnityEngine;

public class InputLineNot : UdonSharpBehaviour
{
    public NotGate notGate;
    float countDownTimer;
    public float timeDelayToUpdate = 0.1f;
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
    // start timer to send the update
    public void UpdateGate()
    {
        startedTimer = true;
    }
    void SendUpdate()
    {
        if (inputSignal)
        {
            notGate.OnTrue();
        }
        else
        {
            notGate.OnFalse();
        }
        notGate.OnPickupUseDown();
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
