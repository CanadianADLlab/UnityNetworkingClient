using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    /// <summary>
    /// Singleton class that manages positions of controllers and cameras
    /// </summary>
    public class XRPositionManager : MonoBehaviour
    {
        public Transform LeftHand;
        public Transform RightHand;
        public GameObject PlaySpace;
        public GameObject CameraOffset;
        public GameObject Camera;


        /// <summary>
        /// Handles weird offset stuff when moving the cameras
        /// </summary>
        /// <param name="portal"></param>
        public void SetRotation(Transform portal)
        {
            // I wrote this code years ago, it works don't ask questions why it just does
        
            Transform trackingSpace = Camera.transform.parent;
            Transform oldParent = trackingSpace.parent;

            trackingSpace.parent = null;
            PlaySpace.transform.eulerAngles = new Vector3(PlaySpace.transform.eulerAngles.x, Camera.transform.eulerAngles.y, PlaySpace.transform.eulerAngles.z);

            trackingSpace.parent = oldParent;
            PlaySpace.transform.eulerAngles = new Vector3(PlaySpace.transform.eulerAngles.x, portal.eulerAngles.y, PlaySpace.transform.eulerAngles.z);
        }

      

        #region Singleton
        public static XRPositionManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("XRPosition manager already exist destroying the position manager attacthed to : " + transform.name);
                Destroy(this);
            }
        }

        #endregion
    }
}