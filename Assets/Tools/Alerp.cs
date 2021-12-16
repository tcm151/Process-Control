using System;
using System.Threading.Tasks;
using UnityEngine;


namespace ProcessControl.Tools
{
    public static class Alerp
    {
        //> EXECUTE AND ACTION OVER AN INTERVAL OF TIME
        public static async Task ContinuousAction(float duration, Action<float> action)
        {
            float elapsedTime = -Time.deltaTime;
            while ((elapsedTime += Time.deltaTime) < duration)
            {
                action(elapsedTime);
                await Task.Yield();
            }
        }

        //> WAIT FOR DURATION IN SECONDS
        public static async Task Delay(float duration)
        {
            float elapsedTime = -Time.deltaTime;
            while ((elapsedTime += Time.deltaTime) < duration)
                await Task.Yield();
        }

        //> EXECUTE AN ACTION AFTER DURATION SECONDS
        public static async void DelayedAction(float duration, Action action)
        {
            float elapsedTime = -Time.deltaTime;
            while ((elapsedTime += Time.deltaTime) < duration)
                await Task.Yield();
            
            action.Invoke();
        }
    }
}