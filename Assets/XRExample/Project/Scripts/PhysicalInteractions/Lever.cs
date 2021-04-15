using EpicXRCrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    public class Lever : Slider
    {

        [Header("Config")]
        public ButtonTypes Button = ButtonTypes.Trigger;
        [SerializeField] AnimationCurve curve;

        [Header("SpringBack")]
        [SerializeField] bool springBackToRestRatio = true;
        [SerializeField] [Range(0f, 1f)] float restRatio = 0.5f;
        [SerializeField] float springPower = 10f;

        [Header("Output")]
        public float RatioOutput;

        Vector3 minPos, maxPos;
        Transform sliderTrans;
        float restXValue;
        bool leftHandInUse = false;

        private void Start()
        {
            minPos = transform.Find("Bounds").Find("Min").localPosition;
            maxPos = transform.Find("Bounds").Find("Max").localPosition;

            sliderTrans = transform.Find("GrabPoint");

            if (maxPos.x - minPos.x == 0)
            {
                print("maxPos.x - minPos.x cannot be zero");
                restXValue = 0;
            }
            else
            {
                restXValue = restRatio * (maxPos.x - minPos.x) + minPos.x;
            }
        }

        private void Update()
        {
            Slide();
            Clamp();
            EvaluateRatio();
        }
        void Clamp()
        {
            EvaluateRatio();

            if (sliderTrans.localPosition.x < minPos.x)
            {
                sliderTrans.localPosition = new Vector3(minPos.x, sliderTrans.localPosition.y, sliderTrans.localPosition.z);
            }
            else if (sliderTrans.localPosition.x > maxPos.x)
            {
                sliderTrans.localPosition = new Vector3(maxPos.x, sliderTrans.localPosition.y, sliderTrans.localPosition.z);
            }

            sliderTrans.localPosition = new Vector3(sliderTrans.localPosition.x, 0 + curve.Evaluate(RatioOutput), sliderTrans.localPosition.z);
        }

        void Slide()
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
            }

            if (!_inUse && springBackToRestRatio)
            {
                EvaluateRatio();
                if (RatioOutput != restRatio)
                {
                    sliderTrans.localPosition = new Vector3(Mathf.MoveTowards(sliderTrans.localPosition.x, restXValue, Time.deltaTime * springPower), sliderTrans.localPosition.y, sliderTrans.localPosition.z);
                }
            }
        }

        void TrackHand(bool _isLeftHand)
        {
            if (_isLeftHand)
            {
                MoveSliderToTransform(XRPositionManager.Instance.LeftHand);
            }
            else
            {
                MoveSliderToTransform(XRPositionManager.Instance.RightHand);
            }
        }

        void MoveSliderToTransform(Transform _trans)
        {
            //put slider in pos
            sliderTrans.position = _trans.position;

            //zero out y and z
            sliderTrans.localPosition = new Vector3(sliderTrans.localPosition.x, 0, 0);
        }
        void EvaluateRatio()
        {
            if (maxPos.x - minPos.x == 0)
                return;

            RatioOutput = (sliderTrans.localPosition.x - minPos.x) / (maxPos.x - minPos.x);
        }

    }
}