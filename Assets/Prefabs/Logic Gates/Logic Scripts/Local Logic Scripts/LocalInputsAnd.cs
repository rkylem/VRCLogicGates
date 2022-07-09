using UdonSharp;
using UnityEngine;

public class LocalInputsAnd : UdonSharpBehaviour
{
    public LocalAndGate andGate;

    public float timeDelayToUpdate = 0.2f;
    float countDownTimer;
    bool startedTimer = false;

    public bool isNand = false;

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
        if (inputA && inputB)
        {
            if (isNand)
            {
                andGate.OnFalse();
            }
            else
            {
                andGate.OnTrue();
            }
        }
        else
        {
            if (isNand)
            {
                andGate.OnTrue();
            }
            else
            {
                andGate.OnFalse();
            }
        }
    }
    void SendUpdate()
    {
        if (aInUse || bInUse)
        {
            if (inputA && inputB)
            {
                if (isNand)
                {
                    andGate.OnFalse();
                }
                else
                {
                    andGate.OnTrue();
                }
            }
            else
            {
                if (isNand)
                {
                    andGate.OnTrue();
                }
                else
                {
                    andGate.OnFalse();
                }
            }
        }
    }
}
