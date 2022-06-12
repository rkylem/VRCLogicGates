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

    // If player moves the Gate, disconnect
    public override void OnPickup()
    {
        //if (input)
        //{
        //    input.SetInUse(false);
        //    input.SetInputSignal(false);
        //    input.ForceUpdateGate();
        //}
        pick();
    }
    public void pick()
    {
        if (input)
        {
            // figured out what is happening, we never fully disconnect the last powerline
            // that was connected, and when we connect another powerline, that should overwrite 
            // the old one, but it seems like it's not doing just that, I think when we connect
            // a new powerline we need to add some code stuff there.
            // still unsure if this will work
            // okay, only when it's connected to its self will it need to be manually disconnected?
            // because we don't want to disconnect from whatever it is connected to when the powerline 
            // didn't move
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
    }

    public override void OnPickupUseDown()
    {// seems like these netowrk even can only call public functions
        // Plan on making this convert the object into a buffer in this case
        // make sure not to break the switch it also uses this code here
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