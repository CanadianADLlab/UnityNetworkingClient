using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EpicXRCrossPlatformInput
{
    public class EX_LightControl : MonoBehaviour
    {
        [SerializeField] List<Light> lights;
        [SerializeField] SingleAxisSlider slider;


        void Update()
        {
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].intensity = slider.RatioOutput * 2;
            }
        }
    }
}