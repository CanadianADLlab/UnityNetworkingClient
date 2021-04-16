using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    /// <summary>
    /// Handles the logic of objects being grabbed
    /// </summary>

    [RequireComponent(typeof(Rigidbody))]
    public class InteractableObject : MonoBehaviour
    {
        [Header("Grab Settings")]
        public int ControllerLayer = 12;

        public bool IsGrabEnabled = true;
   
        public bool HideControllerOnGrab = false;
        public bool HoldToGrab = true;

        public ButtonTypes GrabButton = ButtonTypes.Grip;

        [Header("Controller Snap Settings")]
        public bool SnapToController = true;
        public bool SnapTonController2Hand = false;

        [Header("Throw Settings")]
        public float ThrowVelocityMultiplier = 1; // Multiplies the vel of a throw 
        public float ThrowTorqueMultiplier = 1; // Multiplies the torque of a throw (Torque is rotation.... I didn't know what that ment like a year ago so you know in case I forget again)

        // Hiden bool values
        [HideInInspector]
        public bool IsGrabbedLeft = false;
        [HideInInspector]
        public bool IsGrabbedRight = false;
        [HideInInspector]
        public bool IsGrabbed = false; // Used to make sure we cant grab it out of our hand
        [HideInInspector]
        public bool WasGrabbed = false; // True if held last frame
        [HideInInspector]
        public bool WaitingToGrab = false; // True if held last frame

        private Transform oldParent;

        private Vector3 velocity = Vector3.zero;
        private Vector3 previousVelocity = Vector3.zero;

        private Vector3 torque = Vector3.zero;
        private Vector3 previousTorque = Vector3.zero;

        private Rigidbody rb;
        private bool isOldParentSet = false;

        private bool rightButtonPressed = false;
        private bool leftButtonPressed = false;

        private Controller rightController;
        private Controller leftController;

        private bool oldColliderTrigger = false;

        private bool initalized = false;

        private void Init() // Move from start to init because unity is fucked
        {
            if (!initalized && XRPositionManager.Instance)
            {
                print("we're never setting this are we");
                leftController = XRPositionManager.Instance.LeftHand.GetComponent<Controller>();
                rightController = XRPositionManager.Instance.RightHand.GetComponent<Controller>();
                rb = GetComponent<Rigidbody>();
                if (rb == null)
                {
                    Debug.LogError("No rigidbody attatched please attatch one in the inspector");
                }
                initalized = true;
            }
        }

        private void UpdateGrabButton()
        {
            leftButtonPressed = XRCrossPlatformInputManager.Instance.GetInputByButton(GrabButton,ControllerHand.Left,HoldToGrab);
            rightButtonPressed = XRCrossPlatformInputManager.Instance.GetInputByButton(GrabButton, ControllerHand.Right, HoldToGrab);
        }

        private void Update()
        {
            if (IsGrabEnabled && XRCrossPlatformInputManager.Instance )
            {
                Init();
                CheckWasGrabbed();
                UpdateGrabButton();
                CalculatedAverageVelocity();
            }
        }


        /// <summary>
        /// Handles updating the controllers on whats going on
        /// </summary>
        private void UpdateController(Controller contr,bool updateValue)
        {
            contr.IsGrabbing = updateValue;
        }
        /// <summary>
        /// Calculates the velocity of the last 10 frames
        /// </summary>
        private void CalculatedAverageVelocity()
        {
            // vel
            velocity = ((transform.position - previousVelocity)) / Time.deltaTime;
            previousVelocity = transform.position;

            // torque
            torque = ((transform.localEulerAngles - previousVelocity)) / Time.deltaTime;
            previousTorque = transform.localEulerAngles;
        }

        public Vector3 GetVelocity()
        {
            return velocity;
        }

        private void CheckReleaseGrab()
        {
            if (HoldToGrab)
            {
                if (!leftButtonPressed && IsGrabbedLeft && IsGrabbed)
                {
                    ReleaseGrabLeft();
                }

                if (!rightButtonPressed && IsGrabbedRight && IsGrabbed)
                {
                    ReleaseGrabRight();
                }
            }
            else
            {
                if (leftButtonPressed && IsGrabbedLeft && IsGrabbed)
                {
                    ReleaseGrabLeft();
                }

                if (rightButtonPressed && IsGrabbedRight && IsGrabbed)
                {
                    ReleaseGrabRight();
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (IsGrabEnabled)
            {
                CheckGrab(other);
            }
        }

        public void CheckGrab(Collider other) // this is the ontrigger function moved so it can be called from other scripts
        {
            if (other.gameObject.layer == ControllerLayer)
            {
                if (!IsGrabbed)
                {
                    if (HoldToGrab)
                    {
                        if (leftButtonPressed && other.tag.Equals("LeftController") && !leftController.IsBeingUsed) // Making sure only the left can grab stuff if its the collision
                        {
                            if (!IsGrabbedLeft)
                            {
                                leftController.IsBeingUsed = true;
                                IsGrabbedLeft = true;
                                UpdateController(leftController, IsGrabbedLeft);
                                AttatchToController(other, XRPositionManager.Instance.LeftHand.gameObject);
                            }
                        }

                        if (rightButtonPressed && other.tag.Equals("RightController") && !rightController.IsBeingUsed) // Making sure only the right can grab stuff if its the collision
                        {
                            if (!IsGrabbedRight)
                            {
                                rightController.IsBeingUsed = true;
                                IsGrabbedRight = true;
                                UpdateController(rightController, IsGrabbedRight);
                                AttatchToController(other, XRPositionManager.Instance.RightHand.gameObject);
                            }
                        }
                    }
                    else
                    {
                        if (leftButtonPressed && other.tag.Equals("LeftController") && !leftController.IsBeingUsed) // Making sure only the left can grab stuff if its the collision
                        {
                            if (!IsGrabbedLeft)
                            {
                                WaitingToGrab = true;
                                StartCoroutine(WaitToGrabLeft(other)); // waits till end of frame to set value to true
                            }
                        }

                        if (rightButtonPressed && other.tag.Equals("RightController") && !rightController.IsBeingUsed) // Making sure only the right can grab stuff if its the collision
                        {
                            if (!IsGrabbedRight)
                            {
                                WaitingToGrab = true;
                                StartCoroutine(WaitToGrabRight(other));
                            }
                        }
                    }

                }
                CheckReleaseGrab(); // Might need to be changed if we don't have collision grabbing (Although when would that ever happen)
            }
        }

        /// <summary>
        /// Used to check if object was being grabbed last frame
        /// </summary>
        private void CheckWasGrabbed()
        {
            if (WasGrabbed && !IsGrabbed)
            {
                WasGrabbed = false;
            }
        }

        private IEnumerator WaitToGrabLeft(Collider other)
        {
            yield return new WaitForEndOfFrame();
            leftController.IsBeingUsed = true;
            IsGrabbedLeft = true;
            UpdateController(leftController, IsGrabbedLeft);
            AttatchToController(other, XRPositionManager.Instance.LeftHand.gameObject);
            WaitingToGrab = false;
        }

        private IEnumerator WaitToGrabRight(Collider other)
        {
            yield return new WaitForEndOfFrame();
            rightController.IsBeingUsed = true;
            IsGrabbedRight = true;
            UpdateController(rightController, IsGrabbedRight);
            AttatchToController(other, XRPositionManager.Instance.RightHand.gameObject);
            WaitingToGrab = false;
        }
        /// <summary>
        /// Sets all the values of the object when we attatch to the controller
        /// </summary>
        public void AttatchToController(Collider other, GameObject controller)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            IsGrabbed = true;
            WasGrabbed = true;

            if (!isOldParentSet)
            {
                oldParent = transform.parent;
                isOldParentSet = true;
            }
            transform.parent = other.transform;
            if (SnapToController) // This should be changed for the 1 handed weapons deffinitly a bad way of doing this
            {
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
            }
            if (SnapTonController2Hand)
            {
                transform.forward = controller.transform.forward;
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = new Vector3(270.0f, 180.0f, 0.0f);
            }

            if (transform.GetComponent<Collider>())
            {
                oldColliderTrigger = transform.GetComponent<Collider>().isTrigger;
                transform.GetComponent<Collider>().isTrigger = true;
            }
            // Controller is only hidden if controller hidden bool is true
            HideController(controller);
        }

        private void Release()
        {
            rb.useGravity = true;
            IsGrabbed = false;

            if (oldParent != null) // The old parent can not ever be a holster
            {
                transform.parent = null;
            }
            else
            {
                transform.parent = oldParent;
            }

            rb.isKinematic = false;

            if (transform.GetComponent<Rigidbody>())
            {
                transform.GetComponent<Rigidbody>().velocity = velocity * ThrowVelocityMultiplier; // Adding velocity 
                transform.GetComponent<Rigidbody>().AddTorque((torque * ThrowTorqueMultiplier)); // Adding velocity 
            }
            if (transform.GetComponent<Collider>())
            {
                transform.GetComponent<Collider>().isTrigger = oldColliderTrigger;
            }
            isOldParentSet = false; // When we are done we don't need to know what the old parent is
        }
        private void ReleaseGrabRight()
        {
            rightController.IsBeingUsed = false;
            rightController.IsGrabbing = false;
            IsGrabbedRight = false;
            Release();
            ShowController(XRPositionManager.Instance.RightHand.gameObject);
        }
        private void ReleaseGrabLeft()
        {
            // Hand Specific stuff
            leftController.IsBeingUsed = false;
            leftController.IsGrabbing = false;
            IsGrabbedLeft = false;
            Release();
            ShowController(XRPositionManager.Instance.LeftHand.gameObject);
        }


        public void HideController(GameObject controller)
        {
            if (HideControllerOnGrab)
            {
                if (controller.GetComponent<Controller>() != null)
                {
                    controller.GetComponent<Controller>().HideController();
                }
                else
                {
                    Debug.LogError("No Controller script attatched to the controller object :" + controller.name);
                }
            }
        }

        public void ShowController(GameObject controller)
        {
            if (HideControllerOnGrab)
            {
                if (controller.GetComponent<Controller>() != null)
                {
                    controller.GetComponent<Controller>().ShowController();
                }
                else
                {
                    Debug.LogError("No Controller script attatched to the controller object :" + controller.name);
                }
            }
        }


        /// <summary>
        /// Used to force dropping
        /// </summary>
        public void ReleaseGrab()
        {
            if (IsGrabbedLeft)
            {
                ReleaseGrabLeft();
            }
            else if (IsGrabbedRight)
            {
                ReleaseGrabRight();
            }

        }

        /// <summary>
        /// Makes sure everything is reset correctly than destorys itself
        /// </summary>
        public void MurderThanDestroy() // good function name I know
        {
            StartCoroutine(WaitToDestroy());
        }

        private IEnumerator WaitToDestroy()
        {
            IsGrabEnabled = false;
            ReleaseGrab();

            while(IsGrabbed)
            {
                yield return null;
            }
            Destroy(this.gameObject);
        }
    }
}