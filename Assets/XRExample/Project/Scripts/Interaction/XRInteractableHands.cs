using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EpicXRCrossPlatformInput;

namespace EpicXRCrossPlatformInput
{
    /// <summary>
    /// Attactch with interactable objects to hide handss
    /// </summary>

    [RequireComponent(typeof(InteractableObject))]
    public class XRInteractableHands : MonoBehaviour
    {
        public GameObject LeftHandMesh;
        public GameObject RightHandMesh;

        public bool IsTwoHands = false;

        [DrawIf("IsTwoHands", true)]  // the user doesn't need to see these if no hands
        public GameObject SecondaryLeftHandMesh;
        [DrawIf("IsTwoHands", true)]
        public GameObject SecondaryRightHandMesh;


        private InteractableObject interactable;
        private InteractableSecondaryGrab secondaryInteractable; // not needed but some things might want to be two handed

        private bool leftHandOn = false;
        private bool rightHandOn = false;
        private bool firstHandOn = false;
        private bool secondaryHandOn = false;
        private Controller secondaryGrabbingController;

        private bool leftHandWasOn = false;
        private bool rightHandWasOn = false;




        void Start()
        {
            interactable = GetComponent<InteractableObject>();
            secondaryInteractable = GetComponent<InteractableSecondaryGrab>();
            DisableMeshes();
        }

        private void DisableMeshes()
        {
            if (LeftHandMesh == null || RightHandMesh == null)
            {
                Debug.LogError("No meshs to disabled on XRInteractHand on  : " + this.transform);
            }
            LeftHandMesh.SetActive(false);
            RightHandMesh.SetActive(false);
            if (IsTwoHands && secondaryInteractable == null)
            {
                Debug.LogError("No Secondary grab attatched to an object with secondary grab hands on please attatch or disable on : " + this.transform);
            }
            if (IsTwoHands)
            {
                SecondaryLeftHandMesh.SetActive(false);
                SecondaryRightHandMesh.SetActive(false);
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            TurnOnHand(other);
        }
        private void OnTriggerStay(Collider other)
        {
            TurnOnHand(other);
        }


        private void TurnOnHand(Collider other)
        {
            if (other.gameObject.layer == interactable.ControllerLayer)
            {
                Controller xrGamepad = other.GetComponent<Controller>();
                if (!firstHandOn)
                {
                    if (xrGamepad.Hand == ControllerHand.Left && !rightHandOn)
                    {
                        leftHandOn = true;
                        firstHandOn = true;
                        xrGamepad.HideController();
                        StartCoroutine(WaitToShowController(LeftHandMesh));
                    }
                    else if (xrGamepad.Hand == ControllerHand.Right && !leftHandOn)
                    {
                        rightHandOn = true;
                        firstHandOn = true;
                        xrGamepad.HideController();
                        StartCoroutine(WaitToShowController(RightHandMesh));
                    }
                }
                else if (IsTwoHands && interactable.IsGrabbed) // if already being primary held we check to make sure its a two hander before turning on the other hand
                {
                    if (xrGamepad.Hand == ControllerHand.Left && !leftHandOn && !XRPositionManager.Instance.LeftHand.GetComponent<Controller>().IsGrabbing) // Make sure the left hand isn't already holding item
                    {
                        secondaryHandOn = true;
                        secondaryGrabbingController = null;
                        ToggleHand(SecondaryLeftHandMesh, true);
                        xrGamepad.HideController();
                    }
                    else if (xrGamepad.Hand == ControllerHand.Right && !rightHandOn && !XRPositionManager.Instance.RightHand.GetComponent<Controller>().IsGrabbing)
                    {
                        secondaryHandOn = true;
                        secondaryGrabbingController = null;
                        ToggleHand(SecondaryRightHandMesh, true);
                        xrGamepad.HideController();
                    }
                }
            }
        }

        private IEnumerator WaitToShowController(GameObject controller)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            ToggleHand(controller, true);
        }

        public void FixedUpdate()
        {
            ShowRightHand();
            ShowLeftHand();
            ShowSecondHand();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == interactable.ControllerLayer)
            {
                Controller xrGamepad = other.GetComponent<Controller>();
                if (!interactable.IsGrabbed)
                {
                    if (xrGamepad.Hand == ControllerHand.Left)
                    {
                        ToggleHand(LeftHandMesh, false);
                        leftHandOn = false;
                        xrGamepad.ShowController();
                        firstHandOn = false;
                    }
                    else if (xrGamepad.Hand == ControllerHand.Right)
                    {
                        ToggleHand(RightHandMesh, false);
                        rightHandOn = false;

                        xrGamepad.ShowController();
                        firstHandOn = false;
                    }
                }
                else if (IsTwoHands) // if already being primary held we check to make sure its a two hander before turning on the other hand
                {
                    secondaryGrabbingController = xrGamepad; // this is for a weird edge case if the users hand leaves the trigger while still grabbing
                    if (!secondaryInteractable.IsGrabbed)
                    {
                        if (xrGamepad.Hand == ControllerHand.Left && !leftHandOn)
                        {
                            secondaryHandOn = false;
                            secondaryGrabbingController = null;
                            ToggleHand(SecondaryLeftHandMesh, false);
                            xrGamepad.ShowController();
                        }
                        else if (xrGamepad.Hand == ControllerHand.Right && !rightHandOn)
                        {
                            secondaryHandOn = false;
                            secondaryGrabbingController = null;
                            ToggleHand(SecondaryRightHandMesh, false);
                            xrGamepad.ShowController();
                        }
                    }
                }
            }
        }



        private void ShowLeftHand()
        {
            if (leftHandOn || leftHandWasOn)
            {
                leftHandWasOn = true;
                if (LeftHandMesh.activeSelf)
                {
                    if (!secondaryHandOn && !firstHandOn)
                    {
                        print("Turning off");
                        ToggleHand(LeftHandMesh, false);
                        XRPositionManager.Instance.LeftHand.GetComponent<Controller>().ShowController();
                        leftHandWasOn = false;
                    }
                }
            }
        }





        private void ShowRightHand()
        {
            if (rightHandOn || rightHandWasOn)
            {
                rightHandWasOn = true;
                if (RightHandMesh.activeSelf)
                {
                    if (!secondaryHandOn && !firstHandOn)
                    {
                        ToggleHand(RightHandMesh, false);
                        XRPositionManager.Instance.RightHand.GetComponent<Controller>().ShowController();
                        rightHandWasOn = false;
                    }
                }
            }
        }

        /// <summary>
        /// This function is for an edge case where the user pulls their hand out of the trigger while still grabbing
        /// </summary>
        private void ShowSecondHand()
        {
            if (IsTwoHands && !secondaryInteractable.IsGrabbed)
            {
                if (secondaryGrabbingController != null)
                {
                    if (secondaryGrabbingController.Hand == ControllerHand.Left && !leftHandOn)
                    {
                        ToggleHand(SecondaryLeftHandMesh, false);
                        secondaryGrabbingController.ShowController();
                        secondaryGrabbingController = null;
                    }
                    else if (secondaryGrabbingController.Hand == ControllerHand.Right && !rightHandOn)
                    {
                        ToggleHand(SecondaryRightHandMesh, false);
                        secondaryGrabbingController.ShowController();
                        secondaryGrabbingController = null;
                    }
                    else if (firstHandOn && !rightHandOn && secondaryHandOn)
                    {
                        ToggleHand(SecondaryRightHandMesh, false);
                        XRPositionManager.Instance.RightHand.GetComponent<Controller>().ShowController();
                        secondaryGrabbingController = null;
                    }
                    else if (firstHandOn && !leftHandOn && secondaryHandOn)
                    {
                        ToggleHand(SecondaryLeftHandMesh, false);
                        XRPositionManager.Instance.LeftHand.GetComponent<Controller>().ShowController();
                        secondaryGrabbingController = null;
                    }
                }
            }
        }
        private void ToggleHand(GameObject hand, bool onOff)
        {
            hand.SetActive(onOff);
        }
    }
}



