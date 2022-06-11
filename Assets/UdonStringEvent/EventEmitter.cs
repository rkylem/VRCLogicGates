// Emit events!
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

/// <Summary>A Behaviour that can emit events. An Emitter will check if this Emitter has emitted each update.</Summary>
public class EventEmitter : UdonSharpBehaviour
{
    public EventReceiver receiver;

    // If newEvent doesn't match oldEvent, then a new event has been emitted.
    [UdonSynced]
    private string newEvent;
    private string oldEvent;
    private int ownerID;

    private int clock;

    public void Start()
    {
        newEvent = "";
        oldEvent = "";
        clock = 0;
    }

    /// <Summary>Set a new event to be emitted.</Summary>
    public void SetNewEvent(string eventName, string payload)
    {
        Debug.Log($"Sending new event: {eventName}:{payload}");
        newEvent = $"{eventName},{payload},{clock}";
        clock++;

        if (VRCPlayerApi.GetPlayerCount() == 1 || Networking.LocalPlayer == null) {
            CheckForEvent();
        }
    }

    public override void OnPreSerialization()
    {
        CheckForEvent();
    }

    public override void OnDeserialization()
    {
        CheckForEvent();
    }

    private void CheckForEvent()
    {
        if (newEvent == oldEvent || newEvent.IndexOf(",") == -1)
        {
            return;
        }

        oldEvent = newEvent;

        if (Networking.LocalPlayer == null)
        {
            receiver.HandleUpdate(0, newEvent);
        }
        else
        {
            receiver.HandleUpdate(Networking.GetOwner(gameObject).playerId, newEvent);
        }
        
    }
}