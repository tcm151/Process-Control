using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcessControl.Tools
{
    public static class UnityExtensions
    {
        //> LISTS
        public static T Take<T>(this List<T> list, int index)
        {
            var item = list[index];
            list.RemoveAt(index);
            return item;
        }
        
        public static T TakeFirst<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public static T ItemAfter<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);
            index = (index >= list.Count - 1) ? 0 : ++index;
            return list[index];
        }

        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            for (int i = 0; i < array.Length; i++) action(array[i]);
        }

        //> CAMERA
        public static Vector3 MousePosition2D(this Camera camera)
        {
            var worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;
            return worldPosition;
        }
        
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

        public static Vector2Int FloorToInt(this Vector2 v2) => new Vector2Int
        {
            x = Mathf.FloorToInt(v2.x),
            y = Mathf.FloorToInt(v2.y),
        };

        //> IENUMERABLES
        public static List<T> Shuffle<T>(this List<T> list, int seed = 69)
        {
            var rng = new System.Random(seed);
            var n = list.Count;
            
            while (n > 1)
            {
                int k = rng.Next(n--);
                (list[n], list[k]) = (list[k], list[n]);
            }

            return list;
        }

        public static List<T> To2D<T>(this T[,] array)
        {
            var list = new List<T>(array.Length);
            for (int x = 0; x < array.GetLength(0); x++) {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    list.Add(array[x,y]);
                }
            }
            return list;
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
        
        public static void MoveTowards(this ref Vector3 current, Vector3 target, float maxDelta)
            => current = Vector3.MoveTowards(current, target, maxDelta);

        public static Vector3 VectorTo(this Vector3 first, Vector3 second)
            => second - first;

        public static float DistanceTo(this Vector3 first, Vector3 second)
            => Vector3.Distance(first, second);

        public static Vector3 Abs(this  Vector3 vector) => new Vector3
        {
            x = Mathf.Abs(vector.x),
            y = Mathf.Abs(vector.y),
            z = Mathf.Abs(vector.z),
        };

        public static Vector3Int CeilToInt(this Vector3 vector) => new Vector3Int
        {
            x = vector.x.CeilToInt(),
            y = vector.y.CeilToInt(),
            z = vector.z.CeilToInt(),
        };
        
        public static Vector3Int FloorToInt(this Vector3 vector) => new Vector3Int
        {
            x = vector.x.FloorToInt(),
            y = vector.y.FloorToInt(),
            z = vector.z.FloorToInt(),
        };
        
        public static Vector3 DirectionTo(this Vector3 origin, Vector3 target) => (target - origin).normalized;

        //> MATH ON FLOATS
        public static float Clamp(this float value, float min, float max)
            => Mathf.Clamp(value, min, max);
        
        public static float ClampR(this ref float value, float min, float max)
            => value = Mathf.Clamp(value, min, max);

        public static float MoveTowards(this ref float value, float target, float maxDelta)
            => value = Mathf.MoveTowards(value, target, maxDelta);

        public static int FloorToInt(this float value)
            => Mathf.FloorToInt(value);

        public static int CeilToInt(this float value)
            => Mathf.CeilToInt(value);
    }
}