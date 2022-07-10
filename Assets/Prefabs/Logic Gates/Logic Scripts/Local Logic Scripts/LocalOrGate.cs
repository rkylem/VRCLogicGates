﻿using UdonSharp;
using UnityEngine;

public class LocalOrGate : UdonSharpBehaviour
{
    public Material green;
    public Material red;
    public GameObject on;
    public GameObject off;
    public LineRenderer powerLine;
    public LocalPowerlineMover powerlineScript;
    public LocalInputsOR input;

    public LocalPowerlineMover connectedPowerLineScriptA;
    public LocalPowerlineMover connectedPowerLineScriptB;

    // If player moves the Gate, disconnect
    public override void OnPickup()
    {
        powerlineScript.holding = true;
        if (input)
        {
            input.aInUse = false;
            input.bInUse = false;
            input.inputA = false;
            input.inputB = false;
            if (powerlineScript.GetConnectedORInput() == input)
            {
                powerlineScript.OnPickup();
            }
            else
            {
                input.ForceUpdateGate();
            }
        }
        if (connectedPowerLineScriptA)
        {
            connectedPowerLineScriptA.OnPickup();
        }
        if (connectedPowerLineScriptB)
        {
            connectedPowerLineScriptB.OnPickup();
        }
    }
    public override void OnDrop()
    {
        powerlineScript.holding = false;
    }
    public override void OnPickupUseDown()
    {
        ConvertToNor();
    }
    public void ConvertToNor()
    {
        input.isNor = !input.isNor;

        on.SetActive(!on.activeSelf);
        off.SetActive(!off.activeSelf);
        if (on.activeSelf)
        {
            powerLine.material = green;
        }
        else
        {
            powerLine.material = red;
        }
        //send singal over the powerline
        powerlineScript.SendSignalUpdate(on.activeSelf);
    }
    public void OnTrue()
    {
        on.SetActive(true);
        off.SetActive(false);
        powerLine.material = green;

        powerlineScript.SendSignalUpdate(true);
    }
    public void OnFalse()
    {
        on.SetActive(false);
        off.SetActive(true);
        powerLine.material = red;

        powerlineScript.SendSignalUpdate(false);
    }
}