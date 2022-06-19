using UdonSharp;
using UnityEngine;

public class LocalOrGate : UdonSharpBehaviour
{
    public Material green;
    public Material red;
    public GameObject on;
    public GameObject off;
    public LineRenderer powerLine;
    public LocalPowerlineMover powerlineScript;
    public InputsOR input;

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
    }
    public override void OnDrop()
    {
        powerlineScript.holding = false;
    }
    //public override void OnPickupUseDown()
    //{// seems like these netowrk even can only call public functions
    //    // Plan on making this convert the object into a NOR gate in this case    
    //    ConvertToNOR()
    //}
    //public void ConvertToNOR()
    //{
    //    // figure this out later...
    //    on.SetActive(!on.activeSelf);
    //    off.SetActive(!off.activeSelf);
    //    if (on.activeSelf)
    //    {
    //        powerLine.material = green;
    //    }
    //    else
    //    {
    //        powerLine.material = red;
    //    }
    //    // Update gate if pickedup
    //    if (input && !(input.aInUse || input.bInUse))
    //    {
    //        if (input.inputA || input.inputB)
    //        {
    //            input.inputA = on.activeSelf;
    //            input.inputB = on.activeSelf;
    //        }
    //        input.UpdateGate();
    //    }
    //    //send singal over the powerline
    //    powerLineScript.SendSignalUpdate();
    //}
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