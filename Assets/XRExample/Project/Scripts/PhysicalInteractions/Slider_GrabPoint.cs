using EpicXRCrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{

    public class Slider_GrabPoint : MonoBehaviour
    {
        Slider slider;

        private void Start()
        {
            slider = GetComponentInParent<Slider>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 12)//controller layer
            {
                bool _isLeftHand = other.gameObject.GetComponent<Controller>().Hand == ControllerHand.Left;
                if (_isLeftHand)
                {
                    slider.SetLeftHandOver(true);
                }
                else
                {
                    slider.SetRightHandOver(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 12)//controller layer
            {
                bool _isLeftHand = other.gameObject.GetComponent<Controller>().Hand == ControllerHand.Left;
                if (_isLeftHand)
                {
                    slider.SetLeftHandOver(false);
                }
                else
                {
                    slider.SetRightHandOver(false);
                }
            }
        }
    }
}
