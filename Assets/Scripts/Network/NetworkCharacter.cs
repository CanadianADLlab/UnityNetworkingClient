using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour
{
    private Vector3 lastFramePosition = Vector3.zero;
    private Quaternion lastFrameRotation = Quaternion.identity;

    private bool lerping = false;

    private void Awake()
    {
        lastFramePosition = transform.position;
        lastFrameRotation = transform.rotation;
    }
    public void FixedUpdate()
    {
        if ((lastFramePosition != transform.position || lastFrameRotation != transform.rotation) && !lerping)
        {
            ClientSend.SendPlayerValues(transform.position, transform.rotation);
        }

        lastFramePosition = transform.position;
        lastFrameRotation = transform.rotation;
    }


}
