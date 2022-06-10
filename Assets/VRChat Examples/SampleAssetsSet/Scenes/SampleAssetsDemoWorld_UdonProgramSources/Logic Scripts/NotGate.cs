using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class NotGate : UdonSharpBehaviour
{
    public Material green;
    public Material red;
    public GameObject on;
    public GameObject off;
    public LineRenderer powerLine;
    public PowerLineMover powerLineScript;
    public InputLineNot input;

    // If player moves the Gate, disconnect
    public override void OnPickup()
    {
        // the on powerLineScript Onpickup function disconnects the line from the gate,
        // we can use that here
        //InputLineNot input = GetComponentInChildren<InputLineNot>();
        if (input)
        {
            input.SetInUse(false);
            input.SetInputSignal(false);
            input.ForceUpdateGate();            
        }
    }
    //public void TempPickUp()
    //{
    //    InputLineNot input = GetComponentInChildren<InputLineNot>();
    //    if (input)
    //    {
    //        input.SetInUse(false);
    //        input.SetInputSignal(false);
    //        input.UpdateGate();
    //    }
    //}
    public override void OnPickupUseDown()
    {// seems like these netowrk even can only call public functions
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Invert");
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.IsMaster)
        {
            if (on.activeSelf)
            { // might be better to update just the joined player somehow
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnTrue");
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnFalse");
            }
        }
    }
    public void Invert()
    {
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
        // Update gate if pickedup
        //InputLineNot input = GetComponentInChildren<InputLineNot>();
        if (input && !input.GetInUse())
        {
            input.SetInputSignal(on.activeSelf);
            input.UpdateGate();
        }
        //send singal over the powerline
        powerLineScript.SendSignalUpdate();
    }

    // might need to do the Networking.Ismaster for these
    public void NetworkedOnTrue()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnTrue");
    }
    public void NetworkedOnFalse()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnFalse");
    }
    public void OnTrue()
    {
        on.SetActive(true);
        off.SetActive(false);
        powerLine.material = green;

        powerLineScript.SendSignalUpdate();
    }
    public void OnFalse()
    {
        on.SetActive(false);
        off.SetActive(true);
        powerLine.material = red;

        powerLineScript.SendSignalUpdate();
    }
}
