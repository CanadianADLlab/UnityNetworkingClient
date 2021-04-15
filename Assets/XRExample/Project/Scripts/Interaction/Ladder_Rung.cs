using EpicXRCrossPlatformInput;
using UnityEngine;

public class Ladder_Rung : MonoBehaviour
{


    Ladder ladder;
    int rungIndex;

    public void SetLadder(Ladder _ladder, int _rungIndex)
    {
        ladder = _ladder;
        rungIndex = _rungIndex;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)//controller layer
        {
            bool _isLeftHand = other.gameObject.GetComponent<Controller>().Hand == ControllerHand.Left;
            if (_isLeftHand)
            {
                ladder.SetLeftHand(other.gameObject.transform);
            }
            else
            {
                ladder.SetRightHand(other.gameObject.transform);
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
                ladder.SetLeftHand();
            }
            else
            {
                ladder.SetRightHand();
            }
        }
    }

   
}
