using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EpicXRCrossPlatformInput
{

    /// <summary>
    /// Handles teleporting the player around
    /// </summary>
    public class XRCharacterTeleport : MonoBehaviour
    {
        [Header("Control options")]
        public ButtonTypes TeleportButton = ButtonTypes.Stick;

        [Header("Line Renderer Settings")]
        public Material TeleportArrowMat;
        public LineRenderer LineRend;
        public Color ValidColor = Color.blue;
        public Color InvalidColor = Color.red;


        [Header("Teleport Settings ")]
        [Tooltip("The height of the arch from the controller")]
        public float YOffset = 1.0f;
        public float MaxTeleportDistance = 5.0f;
        public LayerMask TeleportRaycastLayers = ~0;



        private Transform[] parabolas;
        private GameObject parabolaParent;
        private bool validTeleport = false;
        private Vector3 validTeleportPosition;
        private Transform teleportPortal;
        private bool stickDown = false;
        private bool teleportPressed = false;

        private ControllerHand hand = ControllerHand.Right;






        private void Start()
        {
            int i = 0;
            hand = GetComponent<Controller>().Hand;
            parabolas = new Transform[GetComponentsInChildren<CurvedLinePoint>().Length];
            parabolaParent = GetComponentInChildren<CurvedLineRenderer>().gameObject; // The parent of all the little parabola objects contain the curvedlinerenderer

            foreach (CurvedLinePoint c in GetComponentsInChildren<CurvedLinePoint>())
            {
                parabolas[i] = c.transform;
                i++;
            }

            if (parabolas == null)
            {
                Debug.LogError("No Curved line points are attatched to this object " + transform.name);
            }
            LineRend = GetComponentInChildren<LineRenderer>();
            if (LineRend == null)
            {
                Debug.LogError("No Curved line are attatched to this object " + transform.name);
            }
            LineRend.startColor = ValidColor;
            LineRend.endColor = ValidColor;
            TeleportArrowMat.SetColor("_BaseColor", ValidColor);
            print("linerend  " + LineRend.name);
                
        }
        public void Update()
        {
            GetInput();
            RaycastCurvedLine();
            TeleportPlayer();
        }

        private void GetInput()
        {
            teleportPressed = XRCrossPlatformInputManager.Instance.GetInputByButton(TeleportButton, hand, true);
        }

        private void TeleportPlayer()
        {
            if (stickDown && !teleportPressed) // Button up
            {
                stickDown = false;
                if (validTeleport)
                {
                    XRPositionManager.Instance.SetRotation(teleportPortal);

                    float yOffset = XRPositionManager.Instance.PlaySpace.transform.position.y - validTeleportPosition.y;

                    Vector3 distanceFromCenterOffset = XRPositionManager.Instance.PlaySpace.transform.position - XRPositionManager.Instance.Camera.transform.position; // The distance the camera rig is to the centre of the room we will use this to make sure the teleport it centered 
                    XRPositionManager.Instance.PlaySpace.transform.position = new Vector3(validTeleportPosition.x, validTeleportPosition.y, validTeleportPosition.z) + new Vector3(distanceFromCenterOffset.x, 0, distanceFromCenterOffset.z);

                }
            }
        }



        private void RaycastCurvedLine()
        {
            if (stickDown || teleportPressed) // On stick down we show the bezier
            {
                stickDown = true;
                parabolaParent.SetActive(true);
                RaycastHit hit;
                if (Physics.Raycast(transform.position
                   , transform.forward, out hit, MaxTeleportDistance, TeleportRaycastLayers))
                {
                    HandleTeleportHit(hit);
                }
                else if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0, 0, MaxTeleportDistance))
                    , Vector3.down, out hit, Mathf.Infinity, TeleportRaycastLayers))
                {
                    HandleTeleportHit(hit);
                }
                else
                {
                    LineRend.startColor = InvalidColor;
                    LineRend.endColor = InvalidColor;
                    TeleportArrowMat.SetColor("_BaseColor", InvalidColor);

                    validTeleport = false;
                    validTeleportPosition = Vector3.zero;
                    parabolaParent.SetActive(false);
                }
            }
            else
            {
                parabolaParent.SetActive(false);
                validTeleport = false;
            }
        }

        private void HandleTeleportHit(RaycastHit hit)
        {
            LineRend.startColor = ValidColor;
            LineRend.endColor = ValidColor;
            TeleportArrowMat.SetColor("_BaseColor", ValidColor);

            parabolas[0].position = transform.position; // The first point of the curve will always be this position

            // Handeling the middle point of the curved beam
            Vector3 offset = transform.position - hit.point;
            parabolas[1].position = transform.position - (offset / 2);

            parabolas[parabolas.Length - 1].position = hit.point; // The final will always be the end of the raycast

            // Portal position and direction
            print(parabolas[parabolas.Length - 1].name);
            teleportPortal = parabolas[parabolas.Length - 1].GetChild(0);
            teleportPortal.position = hit.point;

            Vector3 lookDirection;

            if (hand == ControllerHand.Left)
            {
                lookDirection = new Vector3(XRCrossPlatformInputManager.Instance.LeftStickAxis.x, 0, XRCrossPlatformInputManager.Instance.RightStickAxis.y);
            }
            else
            {
                lookDirection = new Vector3(XRCrossPlatformInputManager.Instance.RightStickAxis.x, 0, XRCrossPlatformInputManager.Instance.RightStickAxis.y);
            }

            Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            teleportPortal.localRotation = Quaternion.RotateTowards(lookRotation, teleportPortal.localRotation, 1);
            teleportPortal.eulerAngles = new Vector3(0, teleportPortal.eulerAngles.y, 0);


            YOffset = Vector3.Distance(parabolas[0].position, parabolas[parabolas.Length - 1].position);
            parabolas[1].position = new Vector3(parabolas[1].position.x, transform.position.y + (YOffset / 2), parabolas[1].position.z);


            validTeleport = true;
            validTeleportPosition = hit.point;
        }
        void OnDrawGizmosSelected()
        {
            // Draws a 5 unit long red line in front of the object
            Gizmos.color = Color.red;
            Vector3 direction = transform.position + transform.TransformDirection(new Vector3(0, 0, MaxTeleportDistance));
            Gizmos.DrawRay(transform.position, direction);
        }


    }
}


