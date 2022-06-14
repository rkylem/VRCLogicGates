using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class LineSplitter : UdonSharpBehaviour
{
    public Material green;
    public Material red;
    public GameObject on;
    public GameObject off;
    public LineRenderer powerLineA;
    public LineRenderer powerLineB;
    public PowerLineMover powerLineAScript;
    public PowerLineMover powerLineBScript;
    public InputLineSplitter input;

    // If player moves the Gate, disconnect
    public override void OnPickup()
    {
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

            if (powerLineAScript.GetConnectedSplitterInput() == input)
            {
                powerLineAScript.OnPickup();
            }
            if (powerLineBScript.GetConnectedSplitterInput() == input)
            {
                powerLineBScript.OnPickup();
            }
            // can save on some performance here some how...
            // maybe instead of calling on Pickup I set the input to null
            // by adding a set input function in the poweline class
            input.inUse = false;
            input.input = false;
            input.ForceUpdateGate();
        }
    }

    public override void OnPickupUseDown()
    {// seems like these netowrk even can only call public functions
        // Plan on making this convert the object into a buffer in this case
        // make sure not to break the switch it also uses this code here

        //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Invert");
        Invert();
    }
    public void Invert()
    {
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
        // Update gate if pickedup
        if (input && !input.inUse)
        {// don't fully understand this.
            input.input  = on.activeSelf;
            input.UpdateGate();
        }
        //send singal over the powerline
        powerLineAScript.SendSignalUpdate(on.activeSelf);
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
