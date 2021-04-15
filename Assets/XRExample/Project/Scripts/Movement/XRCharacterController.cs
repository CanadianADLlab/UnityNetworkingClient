using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{

    public class XRCharacterController : MonoBehaviour
    {

        #region PublicVars
        [Header("Running Settings")]
        public float speed = 5.0f;
        public float maxVelocityChange = 5.0f;



        [Header("Jump Settings")]
        public LayerMask JumpRaycastLayers;
        public float JumpHeight = 5;
        public float FallMultiplier = .50f;
        [Header("Stick To Ground Force")]
        [Range(1, 10)]
        public float StickToGroundForce = 2.0f;


        [Header("Camera")]
        public Camera VrCamera;

        [Header("Other Settings")]
        public Transform PlayerCollider;

        #endregion

        #region PrivateVars
        private bool grounded = false;
        private bool hasJumped = false;
        private Rigidbody rb;


        // Rotation bools
        private bool goodToRotate = false;
        private bool goodToRotateRight = false;
        private bool goodToRotateLeft = false;

        private Transform followTransform;

        private bool ignoringLeftHand = false;
        private bool ignoringRightHand = false;
        // Position
        private Vector3 targetPos;
        private Controller leftController;
        private Controller rightController;

        #endregion

        private void Start()
        {
            leftController = XRPositionManager.Instance.LeftHand.GetComponent<EpicXRCrossPlatformInput.Controller>();
            rightController = XRPositionManager.Instance.RightHand.GetComponent<EpicXRCrossPlatformInput.Controller>();
            followTransform = (new GameObject("FollowCamera")).transform;
            followTransform.transform.rotation = VrCamera.transform.parent.rotation;
            followTransform.parent = VrCamera.transform.parent;
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Fall();
            Move();
        }

        private void Update()
        {
            HandleFollowTransform();
            HandleVRMovement();
            GroundChecker();
            GetMoveInput();
            CheckJump();
            Rotate();
            CheckHoldingItem();
        }

        private void CheckHoldingItem() // This is a new function I added to make sure if an object or item is being held in VR that we ignore the colliders
        {
            if (leftController.IsGrabbing && !ignoringLeftHand)
            {
                IgnoreChildrenColliders(leftController.transform);
                ignoringLeftHand = true;
            }
            else if (ignoringLeftHand && !leftController.IsGrabbing)
            {
                ignoringLeftHand = false;
                IgnoreChildrenColliders(leftController.transform, ignoringLeftHand); // Passing false re enables zee collider
            }
            if (rightController.IsGrabbing && !ignoringRightHand)
            {
                IgnoreChildrenColliders(rightController.transform);
                ignoringRightHand = true;
            }
            else if (ignoringRightHand && !rightController.IsGrabbing)
            {
                ignoringRightHand = false;
                IgnoreChildrenColliders(rightController.transform, ignoringRightHand);
            }
        }

        private void IgnoreChildrenColliders(Transform colliderParentToIgnore, bool ignoreCollider = true)
        {
            foreach (Collider c in colliderParentToIgnore.GetComponentsInChildren<Collider>())
            {
                if (c.name != PlayerCollider.name)
                {
                    Physics.IgnoreCollision(c, PlayerCollider.GetComponent<Collider>(), ignoreCollider);
                }

            }
        }


        private void CheckJump()
        {
            if (grounded)
            {
                // Jump
                if (XRCrossPlatformInputManager.Instance.RightPrimaryButtonDown && !hasJumped)
                {
                    hasJumped = true;
                    DoJump();
                }
            }
        }


        /// <summary>
        /// Not the best name but basically this handles the player moving around the room lets keep everything in check
        /// </summary>
        private void HandleVRMovement()
        {
            PlayerCollider.transform.position = new Vector3(VrCamera.transform.position.x, PlayerCollider.transform.position.y, VrCamera.transform.position.z);
        }
        private void GetMoveInput()
        {
            targetPos = Vector3.zero;

            targetPos = new Vector3(XRCrossPlatformInputManager.Instance.LeftStickAxis.x, XRCrossPlatformInputManager.Instance.LeftStickAxis.y, XRCrossPlatformInputManager.Instance.LeftStickAxis.y); // The if statement gets rid of drift


            targetPos = followTransform.transform.TransformDirection(targetPos);
        }
        private void Move()
        {
            targetPos *= speed;

            // maxVelocityChange *= speed;

            // Apply a force that attempts to reach our target velocity

            Vector3 velocityChange = (targetPos - rb.velocity);

            float maxVelChangeDivider = 1;
            if (!grounded)
            {
                maxVelChangeDivider = 45;
            }
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange / maxVelChangeDivider, maxVelocityChange / maxVelChangeDivider);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange / maxVelChangeDivider, maxVelocityChange / maxVelChangeDivider);
            velocityChange.y = 0;


            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        private void Rotate()
        {
            CheckIfGoodToRotate();

            if (XRCrossPlatformInputManager.Instance.RightStickAxis.x <= -0.9f && goodToRotateLeft)
            {
                RotatePlayer(-90);
                goodToRotate = false;
                goodToRotateLeft = false;
            }
            if (XRCrossPlatformInputManager.Instance.RightStickAxis.x >= 0.9f && goodToRotateRight)
            {
                RotatePlayer(90);
                goodToRotate = false;
                goodToRotateRight = false;
            }
        }

        /// <summary>
        /// Searches for the ground
        /// </summary>
        private void GroundChecker()
        {
            RaycastHit hit;
            if (Physics.SphereCast(PlayerCollider.position - new Vector3(0, PlayerCollider.transform.localScale.y / 2, 0), 0.3f, PlayerCollider.transform.TransformDirection(Vector3.down), out hit, PlayerCollider.parent.transform.localScale.y / 2, JumpRaycastLayers))
            {
                grounded = true;
                hasJumped = false;
            }
            else // Missed the raycast so we're fallinga
            {
                grounded = false;
            }
        }

        private void CheckIfGoodToRotate()
        {
            if (XRCrossPlatformInputManager.Instance.RightStickAxis.x < .1) // If we return to the origin of the joy stick the user is now alloud to rotate again
            {
                goodToRotate = true;
                goodToRotateRight = true;
            }
            if (XRCrossPlatformInputManager.Instance.RightStickAxis.x > -.1) // If we return to the origin of the joy stick the user is now alloud to rotate again
            {
                goodToRotate = true;
                goodToRotateLeft = true;

            }
        }

        /// <summary>
        /// Deals with teleporting the player correctly based of the camera pos not the player area pos
        /// </summary>
        /// <param name="player">The transform of the player object</param>
        /// <param name="destination">The transform of the destination for the player</param>
        private void RotatePlayer(float rotateValue)
        {
            print("Rotating player");
            GameObject temp = new GameObject();
            temp.transform.position = PlayerCollider.transform.position;
            temp.transform.localEulerAngles = transform.localEulerAngles;
            Transform oldParent = transform.parent;
            transform.parent = temp.transform;


            temp.transform.localEulerAngles += new Vector3(0, rotateValue, 0); 

            transform.parent = oldParent;
            Destroy(temp);
        }

        private void DoJump()
        {
            if (hasJumped && grounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, JumpHeight, rb.velocity.z);
            }
        }

        private void Fall()
        {
            if(!grounded)
                {
                if (rb.velocity.y < 0 && rb.velocity.y > -15)
                {
                    rb.velocity += new Vector3(0.0f, -FallMultiplier, 0.0f);
                }
            }
        }


        /// <summary>
        /// we use the follow transform for moving the player in the correct direction, this is needed so player moves at the same speed while looking up
        /// </summary>
        private void HandleFollowTransform()
        {
            followTransform.position = VrCamera.transform.position;
            followTransform.localEulerAngles = new Vector3(0, VrCamera.transform.localEulerAngles.y, 0);
        }

    }
  

}

