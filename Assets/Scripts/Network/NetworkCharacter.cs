using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour
{
    private Vector3 lastFramePosition = Vector3.zero;
    private Quaternion lastframeRotation = Quaternion.identity;
    public void FixedUpdate()
    {
        ClientSend.SendPlayerValues(transform.position, transform.rotation);
    }
}
