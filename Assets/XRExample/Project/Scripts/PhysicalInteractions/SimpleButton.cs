using EpicXRCrossPlatformInput;
using UnityEngine;
using UnityEngine.Events;

namespace EpicXRCrossPlatformInput
{
    [RequireComponent(typeof(AudioSource))]
    public class SimpleButton : MonoBehaviour
    {
        [Header("Config")]
        public ButtonTypes Button = ButtonTypes.Trigger;
        [SerializeField] bool autoClick = false;
        [SerializeField] float cooldownTime = 2f;

        [Header("Output")]
        public UnityEvent DoOneActivate;

        [Header("Optional")]
        [SerializeField] AudioClip clickClip;

        AudioSource source;
        bool leftHandOver = false;
        bool rightHandOver = false;
        bool inCooldown = false;

        private void Start()
        {
            source = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (inCooldown)
                return;

            //lefthand
            if (leftHandOver)
            {
                if (XRCrossPlatformInputManager.Instance.GetInputByButton(Button, ControllerHand.Left, false) || autoClick)
                {
                    Click();
                }
            }

            //righthand
            if (rightHandOver)
            {
                if (XRCrossPlatformInputManager.Instance.GetInputByButton(Button, ControllerHand.Right, false) || autoClick)
                {
                    Click();
                }
            }
        }

        void Click()
        {
            DoOneActivate.Invoke();
            inCooldown = true;
            Invoke("Cooldown", cooldownTime);
            if (clickClip != null)
            {
                source.PlayOneShot(clickClip);
            }
        }

        void Cooldown()
        {
            inCooldown = false;
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