using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class NotGate : UdonSharpBehaviour
{
    public GameObject on;
    public GameObject off;
    public LineRenderer powerLine;
    public Material green;
    public Material red;

    private void Start()
    {
        powerLine.material = red;
    }
    // If player moves the Gate, disconnect
    public override void OnPickup()
    {
        InputLineNot input = GetComponentInChildren<InputLineNot>();
        if (input)
        {
            input.SetInUse(false);
            input.SetInputSignal(false);
        }
    }
    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Inverter");
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.IsMaster)
        {
            if (on.activeSelf)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnTrue");
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnFalse");
            }
        }
    }
    public void Inverter()
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
