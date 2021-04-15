using EpicXRCrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    public class Slider : MonoBehaviour
    {

        protected bool leftHandOver = false;
        protected bool rightHandOver = false;


        public void SetLeftHandOver(bool _leftHandOver)
        {
            leftHandOver = _leftHandOver;
        }

        public void SetRightHandOver(bool _rightHandOver)
        {
            rightHandOver = _rightHandOver;
        }

    }
}