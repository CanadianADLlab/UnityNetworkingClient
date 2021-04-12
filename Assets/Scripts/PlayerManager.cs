using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public  int ID { get; set; } 
    public  string UserName { get; set; }

    public void SetPositionAndRot(Vector3 _pos, Quaternion _rot)
    {
        StartCoroutine(LerpToPosAndRot(_pos, _rot));
    }
    private IEnumerator LerpToPosAndRot(Vector3 _pos, Quaternion _rot)
    {
        float time = 0;
        float duration = .1f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, _rot, time / duration);
            transform.position = Vector3.Lerp(startPosition, _pos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = _pos;
        transform.rotation = _rot;
        yield return null;
    }
}
