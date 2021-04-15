using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ControllerHand
{
    Right,
    Left
}
namespace EpicXRCrossPlatformInput
{
    /// <summary>
    /// Handles functions to do directly with the controllers
    /// </summary>
    public class Controller : MonoBehaviour
    {
        [Tooltip("The literal mesh of the controller")]
        public GameObject ControllerMesh;
        public ControllerHand Hand = ControllerHand.Left;
        public bool IsGrabbing = false;
        public bool IsBeingUsed = false;


        // this used to be a longer function with steamvr because you had to loop through the weird mesh of that
        public void HideController()
        { 
            if (ControllerMesh != null)
            {
                ControllerMesh.SetActive(false);
            }
        }

        public void ShowController()
        {
            if (ControllerMesh != null)
            {
                ControllerMesh.SetActive(true);
            }
        }
    }
}