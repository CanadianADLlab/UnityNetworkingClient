using EpicXRCrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    public class DualAxisSlider : Slider
    {
        [Header("Config")]
        public ButtonTypes Button = ButtonTypes.Trigger;

        [Header("SpringBack")]
        [SerializeField] bool springBackToRestRatio = true;
        [SerializeField] [Range(0f, 1f)] float restXRatio = 0.5f;
        [SerializeField] [Range(0f, 1f)] float restYRatio = 0.5f;
        [SerializeField] float springPower = 10f;

        [Header("Output")]
        public Vector2 RatioOutput;

        Vector3 minXPos, maxXPos, minYPos, maxYPos;
        Transform sliderTrans;
        float restXValue;
        float restYValue;
        bool leftHandInUse = false;


        private void Start()
        {
            minXPos = transform.Find("Bounds").Find("MinX").localPosition;
            maxXPos = transform.Find("Bounds").Find("MaxX").localPosition;
            minYPos = transform.Find("Bounds").Find("MinY").localPosition;
            maxYPos = transform.Find("Bounds").Find("MaxY").localPosition;

            sliderTrans = transform.Find("GrabPoint");

            if (maxXPos.x - minXPos.x == 0)
            {
                print("maxXPos.x - minXPos.x cannot be zero");
                restXValue = 0;
            }
            else
            {
                restXValue = restXRatio * (maxXPos.x - minXPos.x) + minXPos.x;
            }

            if (maxYPos.y - minYPos.y == 0)
            {
                print("maxYPos.y - minYPos.y cannot be zero");
                restYValue = 0;
            }
            else
            {
                restYValue = restYRatio * (maxYPos.y - minYPos.y) + minYPos.y;
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
            //X
            if (sliderTrans.localPosition.x < minXPos.x)
            {
                sliderTrans.localPosition = new Vector3(minXPos.x, sliderTrans.localPosition.y, sliderTrans.localPosition.z);
            }
            else if (sliderTrans.localPosition.x > maxXPos.x)
            {
                sliderTrans.localPosition = new Vector3(maxXPos.x, sliderTrans.localPosition.y, sliderTrans.localPosition.z);
            }


            //Y
            if (sliderTrans.localPosition.y < minYPos.y)
            {
                sliderTrans.localPosition = new Vector3(sliderTrans.localPosition.x, minYPos.y, sliderTrans.localPosition.z);
            }
            else if (sliderTrans.localPosition.y > maxYPos.y)
            {
                sliderTrans.localPosition = new Vector3(sliderTrans.localPosition.x, maxYPos.y, sliderTrans.localPosition.z);
            }
        }

        void EvaluateRatio()
        {
            if (maxXPos.x - minXPos.x == 0)
                return;

            if (maxYPos.y - minYPos.y == 0)
                return;

            RatioOutput.x = (sliderTrans.localPosition.x - minXPos.x) / (maxXPos.x - minXPos.x);
            RatioOutput.y = (sliderTrans.localPosition.y - minYPos.y) / (maxYPos.y - minYPos.y);
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
                if (RatioOutput.x != restXRatio)
                {
                    sliderTrans.localPosition = new Vector3(Mathf.MoveTowards(sliderTrans.localPosition.x, restXValue, Time.deltaTime * springPower), sliderTrans.localPosition.y, sliderTrans.localPosition.z);
                }
                if (RatioOutput.y != restYRatio)
                {
                    sliderTrans.localPosition = new Vector3(sliderTrans.localPosition.x, Mathf.MoveTowards(sliderTrans.localPosition.y, restYValue, Time.deltaTime * springPower), sliderTrans.localPosition.z);
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

            //zero out z
            sliderTrans.localPosition = new Vector3(sliderTrans.localPosition.x, sliderTrans.localPosition.y, 0);
        }
    }
}