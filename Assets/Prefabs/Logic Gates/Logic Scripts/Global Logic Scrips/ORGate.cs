using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class ORGate : UdonSharpBehaviour
{
    public Material green;
    public Material red;
    public GameObject on;
    public GameObject off;
    public LineRenderer powerLine;
    public PowerLineMover powerLineScript;
    public InputsOR input;

    // If player moves the Gate, disconnect
    public override void OnPickup()
    {
        if (input)
        {
            input.aInUse = false;
            input.bInUse = false;
            input.inputA = false;
            input.inputB = false;
            if (powerLineScript.GetConnectedORInput() == input)
            {
                powerLineScript.OnPickup();
            }
            else
            {
                input.ForceUpdateGate();
            }
        }
    }

    //public override void OnPickupUseDown()
    //{// seems like these netowrk even can only call public functions
    //    // Plan on making this convert the object into a NOR gate in this case    
    //    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ConvertToNOR");
    //}

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.IsMaster)
        {
            if (on.activeSelf)
            { // might be better to update just the joined player somehow
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnTrue");
                powerLineScript.SendSignalUpdate(true);
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnFalse");
                powerLineScript.SendSignalUpdate(false);
            }
        }
    }


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

    // might need to do the Networking.Ismaster for these

    public void NetworkedOnTrue()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnTrue");
        powerLineScript.SendSignalUpdate(true);
    }
    public void NetworkedOnFalse()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnFalse");
        powerLineScript.SendSignalUpdate(false);
    }
    public void OnTrue()
    {
        on.SetActive(true);
        off.SetActive(false);
        powerLine.material = green;
    }
    public void OnFalse()
    {
        on.SetActive(false);
        off.SetActive(true);
        powerLine.material = red;
    }
}