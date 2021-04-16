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
    private Quaternion lastFrameRotation = Quaternion.identity;
    private bool lerping = false; // true when the server is moving the object

    private Rigidbody rb;
    private bool startingIsKinematicValue = false;

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

        rb = GetComponent<Rigidbody>();
        if (rb) // set to kinematic at start and wait for the server to tell us we can use physics (object starts falling causes allooot of issues with who moves what)
        {
            startingIsKinematicValue = rb.isKinematic; // use a var for setting back to not kinematic because maybe for some reason theres a rigidbody with this set to true at start
            rb.isKinematic = true; 
        }
        // Register object with gamemanager
        NetworkManager.TrackedObjects.Add(NetworkID, this); // adding this object to the list of tracked objects

        lastFramePosition = transform.position;
        lastFrameRotation = transform.rotation;
    }

    public void SetLocation(Vector3 _pos, Quaternion _rot)
    {
        if (rb) // object has rigidbody
        {
            rb.isKinematic = true; // set it like this so gravity doesn't interfere
        }
        transform.position = _pos;
        transform.rotation = _rot;
        lastFramePosition = _pos;
        lastFrameRotation = _rot;

        if (rb) // object has rigidbody
        {
            rb.isKinematic = startingIsKinematicValue; // set it like this so gravity doesn't interfere
        }
    }
    public void SetPositionAndRot(Vector3 _pos,Quaternion _rot)
    {
        lerping = true;
        StopCoroutine(LerpToPosAndRot(_pos,_rot)); 
        StartCoroutine(LerpToPosAndRot(_pos,_rot));
    }

    private IEnumerator LerpToPosAndRot(Vector3 _pos, Quaternion _rot)
    {
        if (rb) // object has rigidbody
        {
            rb.isKinematic = true; // set it like this so gravity doesn't interfere
        }
        float time = 0;
        float duration = .05f;
     
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        lastFramePosition = startPosition;
        lastFrameRotation = startRotation;
        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, _rot, time / duration);
            transform.position = Vector3.Lerp(startPosition, _pos, time / duration);
            lastFramePosition = transform.position;
            lastFrameRotation = transform.rotation;
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = _pos;
        transform.rotation = _rot;
        lastFramePosition = _pos;
        lastFrameRotation = _rot;
        lerping = false; 
        if (rb) // object has rigidbody
        {
            rb.isKinematic = startingIsKinematicValue; // set it like this so gravity doesn't interfere
        }
    }
    public void FixedUpdate()
    {
        if ((lastFramePosition != transform.position  || lastFrameRotation != transform.rotation) && !lerping)
        {
            ClientSend.SendGameObjectMovedValues(NetworkID, transform.position, transform.rotation);
        }
        lastFramePosition = transform.position;
        lastFrameRotation = transform.rotation;
    }
}
