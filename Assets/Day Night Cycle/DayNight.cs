using ProcessControl.Tools;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


namespace ProcessControl
{
    public class DayNight : Service
    {
        [Header("Lighting")]
        public Light2D sun;
        public AnimationCurve sunCurve;

        [Header("Ticks")]
        [SerializeField, Range(0, 65_536)] internal int ticks;
        [SerializeField] private int morning = 8192;
        
        private const int ONE_DAY = 65_536;

        //> MODIFY BRIGHTNESS BASED UPON CURRENT TIME OF DAY
        private void SetSunIntensity() => sun.intensity = sunCurve.Evaluate(ticks);

        //> INITIALIZE
        protected override void Awake()
        {
            base.Awake();
            ticks = morning;
        }

        //> UPDATE ON CHANGED IN INSPECTOR
        private void OnValidate() => SetSunIntensity();

        //> FIXED TIME STEP
        private void FixedUpdate()
        {
            SetSunIntensity();
            
            ticks++;
            if (ticks >= ONE_DAY) ticks = 0;
        }
    }
}
