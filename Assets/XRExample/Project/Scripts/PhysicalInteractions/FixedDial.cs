using EpicXRCrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    public class FixedDial : MonoBehaviour
    {
        [Header("Config")]
        public ButtonTypes Button = ButtonTypes.Trigger;
        [Range(2, 360)] public int snapPoints = 12;
        [Range(0, 360)] public int maxValue = 180;
        [Header("Output")]
        public float Value = 1;

        bool leftHandOver = false;
        bool rightHandOver = false;
        bool leftHandInUse = false;

        float multipleOf;
        float overUnder;

        private void Start()
        {
            multipleOf = (360 / snapPoints);
            overUnder = (360 + maxValue) / 2;
        }

        private void Update()
        {
            bool _inUse = false;

            if (rightHandOver)
            {
                if (XRCrossPlatformInputManager.Instance.GetInputByButton(Button, ControllerHand.Right, true))
                {
                    _inUse = true;
                    leftHandInUse = false;
                }
            }
            if (!_inUse && leftHandOver)
            {
                if (XRCrossPlatformInputManager.Instance.GetInputByButton(Button, ControllerHand.Left, true))
                {
                    _inUse = true;
                    leftHandInUse = true;
                }
            }

            if (_inUse)
            {
                TrackHand(leftHandInUse);
                CalculateValue();
            }
        }

        void TrackHand(bool _isLeftHand)
        {
            if (_isLeftHand)
            {
                RotateDial(XRPositionManager.Instance.LeftHand);
            }
            else
            {
                RotateDial(XRPositionManager.Instance.RightHand);
            }
        }

        void RotateDial(Transform _trans)
        {
            transform.forward = _trans.up;
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }

        void CalculateValue()
        {
            float _angle = Quaternion.Angle(transform.rotation, transform.parent.rotation);
            float _dot = Vector3.Dot(transform.forward, transform.parent.right);

            Value = _angle;
            if (_dot < 0)
            {
                Value = 360 - _angle;
            }

            Value = Mathf.Round(Value / multipleOf) * multipleOf;

            if (Value == 360)
            {
                Value = 0;
            }

            if (Value > maxValue && Value <= overUnder)
            {
                Value = maxValue;
            }

            if (Value > maxValue && Value > overUnder)
            {
                Value = 0;
            }

            //align with calculated value
            if (Value >= 0)
            {
                transform.localEulerAngles = new Vector3(0, Value, 0);
            }
            else if (Value < 0)
            {
                transform.localEulerAngles = new Vector3(0, Value - 360, 0);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 12)//controller layer
            {
                bool _isLeftHand = other.gameObject.GetComponent<Controller>().Hand == ControllerHand.Left;
                if (_isLeftHand)
                {
                    leftHandOver = true;
                }
                else
                {
                    rightHandOver = true;
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
                    leftHandOver = false;
                }
                else
                {
                    rightHandOver = false;
                }
            }
        }
    }
}