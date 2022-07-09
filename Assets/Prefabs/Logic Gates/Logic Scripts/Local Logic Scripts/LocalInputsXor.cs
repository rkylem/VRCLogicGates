using UdonSharp;
using UnityEngine;

public class LocalInputsXor : UdonSharpBehaviour
{
    public LocalXorGate xorGate;

    public float timeDelayToUpdate = 0.2f;
    float countDownTimer;
    bool startedTimer = false;

    public bool isXnor = false;

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
        if (inputA ^ inputB)
        {
            if (isXnor)
            {
                xorGate.OnFalse();
            }
            else
            {
                xorGate.OnTrue();
            }
        }
        else
        {
            if (isXnor)
            {
                xorGate.OnTrue();
            }
            else
            {
                xorGate.OnFalse();
            }
        }
    }
    void SendUpdate()
    {
        if (aInUse || bInUse)
        {
            if (inputA ^ inputB)
            {
                if (isXnor)
                {
                    xorGate.OnFalse();
                }
                else
                {
                    xorGate.OnTrue();
                }
            }
            else
            {
                if (isXnor)
                {
                    xorGate.OnTrue();
                }
                else
                {
                    xorGate.OnFalse();
                }
            }
        }
    }
}
