using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    public class LeverBar : MonoBehaviour
    {
        Transform lookAtPos;

        private void Start()
        {
            lookAtPos = transform.parent.Find("GrabPoint");
        }

        private void Update()
        {
            transform.LookAt(lookAtPos);
        }
    }
}
