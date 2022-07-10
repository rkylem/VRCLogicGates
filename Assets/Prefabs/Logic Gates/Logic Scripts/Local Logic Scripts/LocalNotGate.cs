using UdonSharp;
using UnityEngine;

public class LocalNotGate : UdonSharpBehaviour
{
    public Material green;
    public Material red;
    public GameObject on;
    public GameObject off;
    public LineRenderer powerLine;
    public LocalPowerlineMover powerLineScript;
    public LocalInputNot input;

    public LocalPowerlineMover connectedPowerLineScript;

    public override void OnPickup()
    {
        powerLineScript.holding = true;
        if (input)
        {
            if (powerLineScript.GetConnectedNOTInput() == input)
            {
                powerLineScript.OnPickup();
            }
            else
            {
                input.inUse = false;
                input.inputSignal = false;
                input.ForceUpdateGate();
            }
        }
        if (connectedPowerLineScript)
        {
            connectedPowerLineScript.OnPickup();
        }
    }

    public override void OnDrop()
    {
        powerLineScript.holding = false;
    }

    public override void OnPickupUseDown()
    {
        if (input)
        {
            input.isInverter = !input.isInverter;
        }
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
        //if (input && !input.inUse)
        //{
        //    input.inputSignal = on.activeSelf;
        //    input.UpdateGate();
        //}
        powerLineScript.SendSignalUpdate(on.activeSelf);
    }

    public void OnTrue()
    {
        on.SetActive(true);
        off.SetActive(false);
        powerLine.material = green;
        powerLineScript.SendSignalUpdate(true);
    }
    public void OnFalse()
    {
        on.SetActive(false);
        off.SetActive(true);
        powerLine.material = red;
        powerLineScript.SendSignalUpdate(false);
    }
}