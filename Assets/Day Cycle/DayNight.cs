using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


namespace ProcessControl
{
    public class DayNight : MonoBehaviour
    {
        public Light2D sun;
        public AnimationCurve sunCurve;

        [SerializeField] public int ticks;

        private void Awake()
        {
            ticks = 8192;
        }

        private void FixedUpdate()
        {
            sun.intensity = sunCurve.Evaluate(ticks);
            
            ticks++;
            if (ticks >= 65_536) ticks = 0;
        }
    }
}
