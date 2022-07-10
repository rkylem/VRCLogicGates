using UdonSharp;
using UnityEngine;

public class LocalSplitter : UdonSharpBehaviour
{
    public Material green;
    public Material red;
    public GameObject on;
    public GameObject off;
    public LineRenderer powerLineA;
    public LineRenderer powerLineB;
    public LocalPowerlineMover powerLineAScript;
    public LocalPowerlineMover powerLineBScript;
    public LocalInputSplitter input;

    public LocalPowerlineMover connectedPowerLineScript;

    public override void OnPickup()
    {
        powerLineAScript.holding = true;
        powerLineBScript.holding = true;
        if (input)
        {
            input.input = false;
            input.inUse = false;
            input.ForceUpdateGate();
            if (powerLineAScript.GetConnectedSplitterInput() == input)
            {
                powerLineAScript.SetSplitterInputNull();
            }
            if (powerLineBScript.GetConnectedSplitterInput() == input)
            {
                powerLineBScript.SetSplitterInputNull();
            }
        }
        if (connectedPowerLineScript)
        {
            connectedPowerLineScript.OnPickup();
        }
    }
    
    public override void OnDrop()
    {
        powerLineAScript.holding = false;
        powerLineBScript.holding = false;
    }
    public override void OnPickupUseDown()
    {
        ConvertToNotSplitter();
    }
    public void ConvertToNotSplitter()
    {
        input.isInverter = !input.isInverter;

        on.SetActive(!on.activeSelf);
        off.SetActive(!off.activeSelf);
        if (on.activeSelf)
        {
            powerLineA.material = green;
            powerLineB.material = green;
        }
        else
        {
            powerLineA.material = red;
            powerLineB.material = red;
        }

        powerLineAScript.SendSignalUpdate(on.activeSelf);
    }

    public void OnTrue()
    {
        on.SetActive(true);
        off.SetActive(false);
        powerLineA.material = green;
        powerLineB.material = green;

        powerLineAScript.SendSignalUpdate(true);
        powerLineBScript.SendSignalUpdate(true);
    }
    public void OnFalse()
    {
        on.SetActive(false);
        off.SetActive(true);
        powerLineA.material = red;
        powerLineB.material = red;

        powerLineAScript.SendSignalUpdate(false);
        powerLineBScript.SendSignalUpdate(false);
    }
}