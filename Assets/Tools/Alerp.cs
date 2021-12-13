using System;
using System.Threading.Tasks;
using UnityEngine;


namespace ProcessControl.Tools
{
    public static class Alerp
    {
        public static async Task Interval(float duration, Action<float> action)
        {
            float elapsedTime = 0f;
            while ((elapsedTime += Time.deltaTime) < duration)
            {
                action(elapsedTime);
                await Task.Yield();
            }
        }
    }
}