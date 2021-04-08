using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keep sends request to the network for anything moved
/// </summary>
public class NetworkGameObject : MonoBehaviour
{
    [Tooltip("ID the server uses to figure out which is which NEEDS TO BE UNIQUE")]
    public int NetworkID = 0;

    private Vector3 lastFramePosition = Vector3.zero;

    private void Awake()
    {
        foreach(var netWorkObject in GameObject.FindObjectsOfType<NetworkGameObject>())
        {
            if(netWorkObject != this && netWorkObject.NetworkID == NetworkID) // we cannot have a duplicate network id
            {
                Debug.LogError("The network id assigned to " + transform.name + " is not unique! It must be different or the server will explode also I'm killing this object good bye");
                Destroy(this.gameObject);
            }
        }

        // Register object with gamemanager
        NetworkManager.TrackedObjects.Add(NetworkID, this); // adding this object to the list of tracked objects

        lastFramePosition = transform.position;
    }

    public void SetPositionAndRot(Vector3 _pos,Quaternion _rot)
    {
        transform.position = _pos;
        transform.rotation = _rot;
        lastFramePosition = transform.position;
    }
    public void FixedUpdate()
    {
        if (lastFramePosition != transform.position)
        {
            ClientSend.SendGameObjectMovedValues(NetworkID, transform.position, transform.rotation);
        }
        lastFramePosition = transform.position;
    }
}
