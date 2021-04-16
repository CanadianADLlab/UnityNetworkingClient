using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour
{
    private Vector3 lastFramePosition = Vector3.zero;
    private Quaternion lastFrameRotation = Quaternion.identity;

    [Header("Optional VR stuff")]
    public Transform LeftHand;
    public Transform RightHand;
    public Transform VRBody;

    // only matters for vr players
    private Vector3 leftHandLastFramePosition = Vector3.zero;
    private Quaternion leftHandLastFrameRotation = Quaternion.identity;

    private Vector3 rightHandLastFramePosition = Vector3.zero;
    private Quaternion rightHandLastFrameRotation = Quaternion.identity;

    private bool lerping = false;

    private void Awake()
    {
        lastFramePosition = transform.position;
        lastFrameRotation = transform.rotation;
    }
    public void FixedUpdate()
    {
        if(Client.Instance.IsVR) // vr fella has hands
        {
            if (((lastFramePosition != VRBody.position || lastFrameRotation != VRBody.rotation) || (leftHandLastFramePosition != LeftHand.position || leftHandLastFrameRotation != LeftHand.rotation) || (rightHandLastFramePosition != RightHand.position || rightHandLastFrameRotation != RightHand.rotation)) && !lerping)
            {
                ClientSend.SendPlayerValues(VRBody.position, VRBody.rotation,LeftHand.position,LeftHand.rotation,RightHand.position,RightHand.rotation);
            }
            leftHandLastFramePosition = LeftHand.position;
            leftHandLastFrameRotation = LeftHand.rotation;

            rightHandLastFramePosition = RightHand.position;
            rightHandLastFrameRotation = RightHand.rotation;

            lastFramePosition = VRBody.position;
            lastFrameRotation = VRBody.rotation;

        }
        else
        {
            if ((lastFramePosition != transform.position || lastFrameRotation != transform.rotation) && !lerping)
            {
                ClientSend.SendPlayerValues(transform.position, transform.rotation);
            }

            lastFramePosition = transform.position;
            lastFrameRotation = transform.rotation;
        }
       
    }


}
