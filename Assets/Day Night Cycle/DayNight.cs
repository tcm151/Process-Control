using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


namespace ProcessControl
{
    public class DayNight : MonoBehaviour
    {
        public Light2D sun;
        public AnimationCurve sunCurve;

        [SerializeField, Range(0, 65_536)] internal int ticks;
        [SerializeField] private int morning = 8192;

        private void SetSunIntensity() => sun.intensity = sunCurve.Evaluate(ticks);

        private void Awake() => ticks = morning;
        private void OnValidate() => SetSunIntensity();

        private void FixedUpdate()
        {
            SetSunIntensity();
            
            ticks++;
            if (ticks >= 65_536) ticks = 0;
        }
    }
}
