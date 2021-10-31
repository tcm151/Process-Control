using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.UI
{
    public class DayNightUI : MonoBehaviour
    {
        public DayNight dayNight;
        [FormerlySerializedAs("dayNight")] public TextMeshProUGUI UI;
        
        private void Update()
        {
            if (dayNight.ticks % 64 == 0)
            {
                UI.text = $"{(dayNight.ticks / 64),4:D4}";
            }
        }
    }
}