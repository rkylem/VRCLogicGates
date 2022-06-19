using UdonSharp;
using UnityEngine;

public class LocalInputsOR : UdonSharpBehaviour
{
    public LocalOrGate orGate;

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
        if (aInUse || bInUse)
        {
            startedTimer = true;
        }
    }
    public void ForceUpdateGate()
    {
        if (inputA || inputB)
        {
            orGate.OnTrue();
        }
        else
        {
            orGate.OnFalse();
        }
    }
    void SendUpdate()
    {
        if (aInUse || bInUse)
        {
            if (inputA || inputB)
            {
                orGate.OnTrue();
            }
            else
            {
                orGate.OnFalse();
            }
        }
    }
}