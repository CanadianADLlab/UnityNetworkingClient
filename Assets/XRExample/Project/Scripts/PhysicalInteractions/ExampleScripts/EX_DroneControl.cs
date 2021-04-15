using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    public class EX_DroneControl : MonoBehaviour
    {
        [SerializeField] DualAxisSlider dirSlider;
        [SerializeField] Lever lever;
        [SerializeField] SingleAxisSlider powerSlider;
        [SerializeField] FixedDial dial;
        [SerializeField] Material freeMat, lockedMat;

        MeshRenderer rend;
        Rigidbody rb;
        float power;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rend = GetComponent<MeshRenderer>();
        }

        private void FixedUpdate()
        {
            rb.drag = (dial.Value / dial.maxValue) * 4;
            rb.AddForce((transform.up * 20) * lever.RatioOutput);
            rb.AddForce((transform.forward * 20) * (dirSlider.RatioOutput.x * 2 - 1));
            rb.AddForce((-transform.right * 20) * (dirSlider.RatioOutput.y * 2 - 1));
        }

        public void ToggleKinematic()
        {
            rb.isKinematic = !rb.isKinematic;

            if (rb.isKinematic)
            {
                rend.material = lockedMat;
            }
            else
            {
                rend.material = freeMat;
            }
        }
    }

}