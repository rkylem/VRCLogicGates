using UdonSharp;
using UnityEngine;

public class LocalInputNot : UdonSharpBehaviour
{
    public LocalNotGate notGate;
    public float timeDelayToUpdate = 0.3f;
    float countDownTimer;
    bool startedTimer = false;

    public bool isInverter = true;

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
            if (isInverter)
            {
                notGate.OnFalse();
            }
            else
            {
                notGate.OnTrue();
            }
        }
        else
        {
            if (isInverter)
            {
                notGate.OnTrue();
            }
            else
            {
                notGate.OnFalse();
            }
        }
    }
    void SendUpdate()
    {
        if (inUse)
        {
            if (inputSignal)
            {
                if (isInverter)
                {
                    notGate.OnFalse();
                }
                else
                {
                    notGate.OnTrue();
                }
            }
            else
            {
                if (isInverter)
                {
                    notGate.OnTrue();
                }
                else
                {
                    notGate.OnFalse();
                }
            }
        }
    }
}