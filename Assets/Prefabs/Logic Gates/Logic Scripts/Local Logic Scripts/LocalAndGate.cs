using UdonSharp;
using UnityEngine;

public class LocalAndGate : UdonSharpBehaviour
{
    public Material green;
    public Material red;
    public GameObject on;
    public GameObject off;
    public LineRenderer powerLine;
    public LocalPowerlineMover powerlineScript;
    public LocalInputsAnd input;

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
            if (powerlineScript.GetConnectedAndInput() == input)
            {
                powerlineScript.OnPickup();
            }
            else
            {
                input.ForceUpdateGate();
            }
        }
    }
    public override void OnDrop()
    {
        powerlineScript.holding = false;
    }
    public override void OnPickupUseDown()
    {
        ConvertToNand();
    }
    public void ConvertToNand()
    {
        input.isNand = !input.isNand;

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
