
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GenericButtonTriggerer : UdonSharpBehaviour
{
    void Update()
    {
        if (Networking.LocalPlayer == null)
        {
            return;
        }

        var handLoc = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftIndexDistal);
        if (handLoc == null)
        {
            handLoc = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
        }

        transform.position = handLoc;
    }
}
