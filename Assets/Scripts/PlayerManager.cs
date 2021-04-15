using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public  int ID { get; set; } 
    public  string UserName { get; set; }

    [Header("Optional VR stuff")]
    public Transform LeftHand;
    public Transform RightHand;


    // non vr
    public void SetPositionAndRot(Vector3 _pos, Quaternion _rot,bool _lerp)
    {
        if (_lerp)
        {
            StartCoroutine(LerpToPosAndRot(_pos, _rot,transform));
        }
        else
        {
            transform.position = _pos;
            transform.rotation = _rot;
        }
    }

    // vr version 
    public void SetPositionAndRot(Vector3 _pos, Quaternion _rot, Vector3 _leftHandPos, Quaternion _leftHandRot, Vector3 _rightHandPos, Quaternion _rightHandRot,bool _lerp)
    {
        if (_lerp)
        {
            StartCoroutine(LerpToPosAndRot(_pos, _rot, transform));
            StartCoroutine(LerpToPosAndRot(_rightHandPos, _rightHandRot, RightHand));
            StartCoroutine(LerpToPosAndRot(_leftHandPos, _leftHandRot, LeftHand));
        }
        else
        {
            transform.position = _pos;
            transform.rotation = _rot;
            LeftHand.position = _leftHandPos;
            LeftHand.rotation = _leftHandRot;
            RightHand.position = _rightHandPos;
            RightHand.rotation = _rightHandRot;
        }


    }
    private IEnumerator LerpToPosAndRot(Vector3 _pos, Quaternion _rot,Transform lerpTransform)
    {
        float time = 0;
        float duration = .1f;
        Vector3 startPosition = lerpTransform.position;
        Quaternion startRotation = lerpTransform.rotation;

        while (time < duration)
        {
            lerpTransform.rotation = Quaternion.Lerp(startRotation, _rot, time / duration);
            lerpTransform.position = Vector3.Lerp(startPosition, _pos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        lerpTransform.position = _pos;
        lerpTransform.rotation = _rot;
        yield return null;
    }
}
