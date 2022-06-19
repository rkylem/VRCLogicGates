using UdonSharp;
using UnityEngine;

public class LocalInputSplitter : UdonSharpBehaviour
{
    public LocalSplitter lineSplitter;

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
        if (input)
        {
            lineSplitter.OnTrue();
        }
        else
        {
            lineSplitter.OnFalse();
        }
    }
    void SendUpdate()
    {
        if (inUse && input)
        {
            lineSplitter.OnTrue();
        }
        else
        {
            lineSplitter.OnFalse();
        }
    }
}