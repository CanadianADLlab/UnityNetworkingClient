using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    /// <summary>
    /// Handles the movement of rotation 
    /// </summary>

    [RequireComponent(typeof(InteractableObject))]
    public class InteractableSecondaryGrab : MonoBehaviour
    {
        [HideInInspector]
        public bool IsGrabbedLeft = false;
        [HideInInspector]
        public bool IsGrabbedRight = false;
        [HideInInspector]
        public bool IsGrabbed = false;


        [Tooltip("Some guns are backwards enable this to make it correct")]
        public bool ReverseGrabLook = false;



        private InteractableObject mainGrab;
        private int controllerLayerMask;
        private Vector3 orginalGrabRotation = Vector3.zero;

        private Controller leftController;
        private Controller rightController;


        private bool rightButtonPressed = false;
        private bool leftButtonPressed = false;

        private bool leftWasPressed = false;
        private bool rightWasPressed = false;




        private void Start()
        {
            leftController = XRPositionManager.Instance.LeftHand.GetComponent<EpicXRCrossPlatformInput.Controller>();
            rightController = XRPositionManager.Instance.RightHand.GetComponent<EpicXRCrossPlatformInput.Controller>();
            mainGrab = GetComponent<InteractableObject>();
            controllerLayerMask = mainGrab.ControllerLayer;
        }


        private void UpdateGrabButton()
        {
            leftButtonPressed = XRCrossPlatformInputManager.Instance.GetInputByButton(mainGrab.GrabButton, ControllerHand.Left, true); // I'm just always gonna make people hold to grab for the secondary it seems to confusing otherwise but idk follow your heart and do whatever my dude
            rightButtonPressed = XRCrossPlatformInputManager.Instance.GetInputByButton(mainGrab.GrabButton, ControllerHand.Right, true); 
        }

        private void Update()
        {
            UpdateGrabButton(); // Just checks for the input of the grab button
            if (IsGrabbed && IsGrabbedRight)
            {
                // Maybe add an offset to move down
                if (ReverseGrabLook)
                {
                    transform.LookAt((2 * transform.position - rightController.transform.position)); // Look backwards
                }
                else
                {
                    transform.LookAt((rightController.transform.position ) );
                }
                transform.position = leftController.transform.position;
            }
            if (IsGrabbed && IsGrabbedLeft)
            {
                if (ReverseGrabLook)
                {
                    transform.LookAt((2 * transform.position - leftController.transform.position) ); // Look backwards
                }
                else
                {
                    transform.LookAt((leftController.transform.position) );
                }
                transform.position = rightController.transform.position;
            }

            if (!mainGrab.IsGrabbed && IsGrabbed)
            {
                ReleaseGrabRight();
                ReleaseGrabLeft();
            }
        }

        private void CheckRelease()
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

        /// <summary>
        /// Handles updating the controllers on whats going on
        /// </summary>
        private void UpdateController(Controller contr, bool updateValue)
        {
            contr.IsGrabbing = updateValue;
        }
        private void DrawDebugRay()
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward));
        }
        private void OnTriggerEnter(Collider other)
        {
            if (rightButtonPressed)
            {
                rightWasPressed = true;
            }
            else
            {
                rightWasPressed = false;
            }
            if(leftButtonPressed)
            {
                leftButtonPressed = true;
            }
            else
            {
                leftButtonPressed = false;
            }

        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == controllerLayerMask)
            {
                if (mainGrab.IsGrabbed) // this can only work if a primary grab has been set
                {
                    if (mainGrab.HoldToGrab)
                    {
                        if (leftButtonPressed && other.tag.Equals("LeftController") && !leftController.IsBeingUsed) // Making sure only the left can grab stuff if its the collision
                        {
                            if (!IsGrabbedLeft)
                            {
                                leftController.IsBeingUsed = true;
                                IsGrabbedLeft = true;
                                UpdateController(leftController, IsGrabbedLeft);
                                OnGrab(XRPositionManager.Instance.LeftHand.gameObject);
                            }
                        }

                        if (rightButtonPressed && other.tag.Equals("RightController") && !rightController.IsBeingUsed) // Making sure only the right can grab stuff if its the collision
                        {
                            if (!IsGrabbedRight)
                            {
                                rightController.IsBeingUsed = true;
                                IsGrabbedRight = true;
                                UpdateController(rightController, IsGrabbedRight);
                                OnGrab(XRPositionManager.Instance.RightHand.gameObject);
                            }
                        }
                    }
           
                }
                CheckRelease();
            }
        }


        private IEnumerator WaitToGrabLeft()
        {
            yield return new WaitForEndOfFrame();
            leftController.IsBeingUsed = true;
            IsGrabbedLeft = true;
            OnGrab(XRPositionManager.Instance.LeftHand.gameObject);
        }



        private IEnumerator WaitToGrabRight()
        {
            yield return new WaitForEndOfFrame();
            rightController.IsBeingUsed = true;
            IsGrabbedRight = true;
            OnGrab(XRPositionManager.Instance.RightHand.gameObject);
        }


        /// <summary>
        /// Called when we grab the object
        /// </summary>
        private void OnGrab(GameObject controller)
        {
            IsGrabbed = true;
            orginalGrabRotation = transform.localEulerAngles;

            mainGrab.HideController(controller);
        }

        private void ReleaseGrabRight()
        {
            transform.localEulerAngles = orginalGrabRotation;
            IsGrabbedRight = false;
            IsGrabbed = false;
            rightController.IsBeingUsed = false;
            rightController.IsGrabbing = false;
            mainGrab.ShowController(XRPositionManager.Instance.RightHand.gameObject);
        }

        private void ReleaseGrabLeft()
        {
            transform.localEulerAngles = orginalGrabRotation;
            IsGrabbedLeft = false;
            IsGrabbed = false;
            leftController.IsBeingUsed = false;
            leftController.IsGrabbing = false;
            mainGrab.ShowController(XRPositionManager.Instance.LeftHand.gameObject);
        }
    }
}