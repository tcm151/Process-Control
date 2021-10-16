using System;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Tools;
using TMPro;
using UnityEngine;


namespace ProcessControl.User_Interface
{
    public class FPS : MonoBehaviour
    {
        private int ticks = 60;
        public int Granularity = 60;
        private List<float> frameTimes;
        private TextMeshProUGUI fps;

        private void Awake()
        {
            frameTimes = new List<float>();
            fps = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (ticks <= 0)
            {
                fps.text = $"{(1 / frameTimes.Average()).FloorToInt()}";
                ticks = Granularity;
                
                if (frameTimes.Count >= Granularity * 4) frameTimes.Clear();
            } 

            frameTimes.Add(Time.unscaledDeltaTime);
            ticks--; 
        }
    }
}