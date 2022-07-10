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
    public InputNOT input;

    public PowerLineMover connectedPowerLineScript;

    // If player moves the Gate, disconnect
    public override void OnPickup()
    {// might need to network this
        //pick();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "pick");
    }
    public void pick()
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
                input.SetInUse(false);
                input.SetInputSignal(false);
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
    {// seems like these netowrk even can only call public functions
        // Plan on making this convert the object into a buffer in this case
        //// make sure not to break the switch it also uses this code here

        //if (Networking.IsMaster)
        //{
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "NetworkInvert");
        //}
        Invert();
    }

    public void NetworkInvert()
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
    }
    public void Invert()
    {
        // Update gate if pickedup
        if (input && !input.GetInUse())
        {
            input.SetInputSignal(on.activeSelf);
            input.UpdateGate();
        }
        //send singal over the powerline
        powerLineScript.SendSignalUpdate(on.activeSelf);
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

    // might need to do the Networking.Ismaster for these
    // I think if I transfer ownership to who picks up the object
    // then use is master might do something.
    public void NetworkedOnTrue()
    {
        //if (Networking.IsMaster)
        //{
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnTrue");
        //}
        powerLineScript.SendSignalUpdate(true);
    }
    public void NetworkedOnFalse()
    {
        //if (Networking.IsMaster)
        //{
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnFalse");
        //}
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