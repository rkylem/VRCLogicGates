using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class KeyEventListener : UdonSharpBehaviour
{
    public KeyboardManager2 keyboard;

    private void Update()
    {
        if (Networking.LocalPlayer != null && Networking.LocalPlayer.IsUserInVR())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            keyboard.Toggle();
        }
    }
}