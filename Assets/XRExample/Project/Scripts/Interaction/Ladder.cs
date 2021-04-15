using EpicXRCrossPlatformInput;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{

    public ButtonTypes ClimbButton = ButtonTypes.Trigger;


    Rigidbody playerRB = null;
    bool climbing = false;

    List<Ladder_Rung> rungs = new List<Ladder_Rung>();

    Transform trackerTransLeft = null;
    Transform trackerTransRight = null;
   
    Vector3 lastPosLeft = Vector3.zero;
    Vector3 lastPosRight = Vector3.zero;

    bool leftHandClimb = true;

    bool isCurrentLadder = false;

    List<Ladder> ladders = new List<Ladder>();

    private void Start()
    {
        playerRB = XRPositionManager.Instance.PlaySpace.GetComponentInChildren<Rigidbody>();
        rungs.AddRange(GetComponentsInChildren<Ladder_Rung>());

        ladders.AddRange(GameObject.FindObjectsOfType<Ladder>());

        for (int i = 0; i < rungs.Count; i++)
        {
            rungs[i].SetLadder(this,i);
        }
    }

    private void Update()
    {
        if (playerRB == null)
            return;
        Climbing();
        Gravity();
    }

    void Gravity()
    {
        if(climbing)
        {
            playerRB.velocity = Vector3.zero;
        }

        //Gravity controll**********************
        if (climbing && playerRB.useGravity && isCurrentLadder)
        {
            playerRB.useGravity = false;
        }
        if (!climbing && !playerRB.useGravity && isCurrentLadder)
        {
            playerRB.useGravity = true;
        }
        //**************************************
        climbing = false;
    }
    void Climbing()
    {
        //trigger down********************************
        if (trackerTransLeft != null)
        {
            if (XRCrossPlatformInputManager.Instance.GetInputByButton(ClimbButton, ControllerHand.Left, false))
            {
                lastPosLeft = trackerTransLeft.position;
                leftHandClimb = true;
                BecomeCurrentLadder();
                climbing = true;
            }
        }


        if (trackerTransRight != null)
        {
            if (XRCrossPlatformInputManager.Instance.GetInputByButton(ClimbButton, ControllerHand.Right, false))
            {
                lastPosRight = trackerTransRight.position;
                leftHandClimb = false;
                BecomeCurrentLadder();
                climbing = true;
            }
        }
        //********************************************

        //trigger hold****************************************
        if (trackerTransLeft != null && leftHandClimb)
        {
            //trigger hold
            if (XRCrossPlatformInputManager.Instance.GetInputByButton(ClimbButton, ControllerHand.Left, true))
            {
                Vector3 _climbVector = trackerTransLeft.position - lastPosLeft;
                XRPositionManager.Instance.PlaySpace.transform.Translate(-_climbVector,Space.World);
                climbing = true;
            }

            lastPosLeft = trackerTransLeft.position;
        }

        if (trackerTransRight != null && !leftHandClimb)
        {
            //trigger hold
            if (XRCrossPlatformInputManager.Instance.GetInputByButton(ClimbButton, ControllerHand.Right, true))
            {
                Vector3 _climbVector = trackerTransRight.position - lastPosRight;
                XRPositionManager.Instance.PlaySpace.transform.Translate(-_climbVector, Space.World);
                climbing = true;
            }

            lastPosRight = trackerTransRight.position;
        }
        //********************************************
    }

    public void SetLeftHand(Transform _leftHandTrans)
    {
        trackerTransLeft = _leftHandTrans;
        lastPosLeft = trackerTransLeft.position;
    }
    public void SetRightHand(Transform _rightHandTrans)
    {
        trackerTransRight = _rightHandTrans;
        lastPosRight = trackerTransRight.position;
    }

    public void SetLeftHand()
    {
        trackerTransLeft = null;
    }
    public void SetRightHand()
    {
        trackerTransRight = null;
    }

    public void SetCurrentLadder(bool _isCurLadder)
    {
        isCurrentLadder = _isCurLadder;
    }

    void BecomeCurrentLadder()
    {
        for (int i = 0; i < ladders.Count; i++)
        {
            ladders[i].SetCurrentLadder(false);
        }
        isCurrentLadder = true;
    }

}
