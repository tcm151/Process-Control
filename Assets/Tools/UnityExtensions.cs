using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcessControl.Tools
{
    public static class UnityExtensions
    {
        //> LAYER MAKS
        public static bool Contains(this LayerMask layerMask, int layer) => (layerMask == (layerMask | (1 << layer)));
        
        //> COROUTINES
        public static void Delay(this MonoBehaviour monoBehaviour, float delay, Action callback)
            => monoBehaviour.StartCoroutine(DelayedCoroutine(delay, callback));

        private static IEnumerator DelayedCoroutine(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        //> VECTOR2
        public static float Angle(this Vector2 direction)
        {
            direction.Normalize();

            var angle = Mathf.Acos(direction.x) * Mathf.Rad2Deg;
            return (direction.y > 0f) ? angle : -angle;
        }
        
        //> IENUMERABLES
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> e)
        {
            var rng = new System.Random();
            var array = e.ToArray();
            var n = array.Length;
            
            while (n > 1)
            {
                int k = rng.Next(n--);
                (array[n], array[k]) = (array[k], array[n]);
            }

            return array;
        }
        
        public static void MoveTowards(this ref Vector2 current, Vector2 target, float maxDelta)
            => current = Vector2.MoveTowards(current, target, maxDelta);

        public static void Lerp(this ref Vector2 current, Vector2 target, float maxDelta)
            => current = Vector2.Lerp(current, target, maxDelta);

        //> VECTOR3
        public static float Angle(this Vector3 direction)
        {
            direction.Normalize();

            var angle = Mathf.Acos(direction.x) * Mathf.Rad2Deg;
            return (direction.y > 0f) ? angle : -angle;
        }

        public static Vector3 DirectionTo(this Vector3 origin, Vector3 target) => (target - origin).normalized;

        //> MATHF
        public static float Clamp(this ref float value, float min, float max)
            => value = Mathf.Clamp(value, min, max);

        public static float MoveTowards(this ref float value, float target, float maxDelta)
            => value = Mathf.MoveTowards(value, target, maxDelta);



    }
}