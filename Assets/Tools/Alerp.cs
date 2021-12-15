using System;
using System.Threading.Tasks;
using UnityEngine;


namespace ProcessControl.Tools
{
    public static class Alerp
    {
        public static async Task ContinualAction(float duration, Action<float> action)
        {
            float elapsedTime = 0f;
            while ((elapsedTime += Time.deltaTime) < duration)
            {
                action(elapsedTime);
                await Task.Yield();
            }
        }

        // public static async Task Interval(float interval, bool condition)
        // {
        //     
        // }

        /// 
        public static async Task ConditionalDelay(float duration, bool condition)
        {
            float elapsedTime = 0f;
            while ((elapsedTime += Time.deltaTime) < duration || !condition)
                await Task.Yield();
        }

        public static async Task Delay(float delay)
        {
            float elapsedTime = 0f;
            while ((elapsedTime += Time.deltaTime) < delay)
                await Task.Yield();
        }

        public static async void DelayedAction(float delay, Action action)
        {
            float elapsedTime = 0f;
            while ((elapsedTime += Time.deltaTime) < delay)
                await Task.Yield();
            
            action.Invoke();
        }
    }
}