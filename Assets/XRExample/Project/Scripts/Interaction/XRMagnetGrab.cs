using System;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    /// <summary>
    /// This script attatches to the controller and handles grabbing things from distance
    /// </summary>
    public class XRMagnetGrab : MonoBehaviour
    {
        [Header("Controls")]
        [Tooltip("Holding this button will enable the ability to aim and grab")]
        public ButtonTypes MagneticGrabInput = ButtonTypes.Primary;
        [Header("Image Prefab")]
        public GameObject GrabIconPrefab; // Little icon that displays on item when raycasted at

        [Header("RayCast Settings")]
        public float GrabDistance = 10; // How far the controller can grab from in mtrs
        public LayerMask GrabLayer;
        public Vector3 RaycastDirection = new Vector3(0, 0, 1);

        [Header("Tractor Beam")]
        public float MinTractorBeamZ = 5.0f; // Min length when the beam is enabled
        public GameObject TractorBeamPrefab;


        private GameObject tractorBeam;
        private Controller xrGamePad;
        private bool isMagneticGrabbing = false;

        private GameObject handSprite;
        private InteractableObject objectToBeGrabbed;

        private void Start()
        {
            xrGamePad = GetComponent<Controller>();
            if (GrabIconPrefab != null)
            {
                handSprite = GameObject.Instantiate(GrabIconPrefab);
                ToggleHandIcon(false);
            }
            ResetBeam();
        }

        private void ResetBeam()
        {
            if (tractorBeam == null)
            {
                tractorBeam = GameObject.Instantiate(TractorBeamPrefab, Vector3.zero, Quaternion.identity, this.transform);
                tractorBeam.transform.localScale = new Vector3(tractorBeam.transform.localScale.x, tractorBeam.transform.localScale.y, MinTractorBeamZ);
                tractorBeam.transform.localRotation = Quaternion.identity;
                tractorBeam.transform.forward = RaycastDirection;
            }
            tractorBeam.transform.localPosition = new Vector3(0, 0, MinTractorBeamZ / 2);
            tractorBeam.SetActive(false);
        }

        private void DrawDebugRay()
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(transform.TransformDirection(RaycastDirection)));
        }
        private void Update()
        {
            GetInput();
            CastRayFromController();
            DrawDebugRay();
            CheckIfGrabbed();
        }



        private void GetInput()
        {
            isMagneticGrabbing = XRCrossPlatformInputManager.Instance.GetInputByButton(MagneticGrabInput, GetComponent<Controller>().Hand, true);
        }

        private void CheckIfGrabbed()
        {
            if (objectToBeGrabbed != null && objectToBeGrabbed.IsGrabbed) // Basically if we are grabbing I don't even want to let users do things
            {
                ToggleHandIcon(false);
            }
        }

        private void CastRayFromController()
        {
            if (isMagneticGrabbing && !xrGamePad.IsGrabbing)
            {
                RaycastHit hit;
                tractorBeam.SetActive(true);
                if (Physics.Raycast(transform.position,transform.TransformDirection(RaycastDirection), out hit, GrabDistance, GrabLayer))
                {
                    // could be the object, could be a parent, could be a child who knows lets be certain though
                    tractorBeam.transform.localScale = new Vector3(tractorBeam.transform.localScale.x, tractorBeam.transform.localScale.y, hit.distance);
                    tractorBeam.transform.localPosition = new Vector3(0, 0, tractorBeam.transform.localScale.z / 2);
                    if (hit.collider.transform.GetComponent<InteractableObject>() != null)
                    {
                        objectToBeGrabbed = hit.collider.transform.GetComponent<InteractableObject>();
                    }
                    else if (hit.collider.transform.GetComponentInChildren<InteractableObject>() != null)
                    {
                        objectToBeGrabbed = hit.collider.transform.GetComponentInChildren<InteractableObject>();
                    }
                    else if (hit.collider.transform.GetComponentInParent<InteractableObject>() != null)
                    {
                        objectToBeGrabbed = hit.collider.transform.GetComponentInParent<InteractableObject>();
                    }


                    if (objectToBeGrabbed != null && objectToBeGrabbed.SnapToController) // we hit a grabable boy and we need to make sure it has snap mode 
                    {
                        ToggleHandIcon(true);
                        objectToBeGrabbed.CheckGrab(GetComponent<Collider>()); // This function handles grabbing, uses the controller collider to attatch

                        if (!objectToBeGrabbed.HoldToGrab && objectToBeGrabbed.IsGrabbed) // if in the above function the user grabs the thing we need to turn off the icon
                        {
                            ToggleHandIcon(false);
                        }
                        handSprite.transform.position = objectToBeGrabbed.transform.position;
                    }
                    else
                    {
                        objectToBeGrabbed = null;
                        ToggleHandIcon(false);
                    }
                }
                else
                {
                    objectToBeGrabbed = null;
                    ToggleHandIcon(false);
                }
            }
            else
            {
                objectToBeGrabbed = null;
                ToggleHandIcon(false);
                ResetBeam();
            }
        }


        private void ToggleHandIcon(bool onOff)
        {
            if (handSprite != null)
            {
                handSprite.SetActive(onOff);
            }
        }

    }

}
