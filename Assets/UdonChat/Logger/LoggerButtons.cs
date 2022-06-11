using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

///<Summary>A Logger for Udon. Attaches to a player's hand if they're in VR, or sits at some point in space if they're in desktop mode.</Summary>
public class LoggerButtons : UdonSharpBehaviour
{
    [Tooltip("True: UdonChat is placed in a static location in the world. False: UdonChat follows the player.")]
    public bool isStaticScreen;

    void Start()
    {
        if ((Networking.LocalPlayer != null && !Networking.LocalPlayer.IsUserInVR()) || isStaticScreen)
        {
            gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (Networking.LocalPlayer != null && Networking.LocalPlayer.IsUserInVR())
        {
            var data = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand);
            transform.SetPositionAndRotation(
                data.position,
                data.rotation
            );
        }
    }
}
