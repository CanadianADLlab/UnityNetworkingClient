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
    private Quaternion lastframeRotation = Quaternion.identity;
    private bool lerping = false; // true when the server is moving the object

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
        lastframeRotation = transform.rotation;
    }

    public void SetPositionAndRot(Vector3 _pos,Quaternion _rot)
    {
        lerping = true;
        StopCoroutine(LerpToPosAndRot(_pos,_rot)); 
        StartCoroutine(LerpToPosAndRot(_pos,_rot));
    }

    private IEnumerator LerpToPosAndRot(Vector3 _pos, Quaternion _rot)
    {
        float time = 0;
        float duration = .05f;
     
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        lastFramePosition = startPosition;
        lastframeRotation = startRotation;
        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, _rot, time / duration);
            transform.position = Vector3.Lerp(startPosition, _pos, time / duration);
            lastFramePosition = transform.position;
            lastframeRotation = transform.rotation;
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = _pos;
        transform.rotation = _rot;
        lastFramePosition = _pos;
        lastframeRotation = _rot;
        lerping = false;
    }
    public void FixedUpdate()
    {
        if ((lastFramePosition != transform.position  || lastframeRotation != transform.rotation) && !lerping)
        {
            ClientSend.SendGameObjectMovedValues(NetworkID, transform.position, transform.rotation);
        }
        lastFramePosition = transform.position;
        lastframeRotation = transform.rotation;
    }
}
