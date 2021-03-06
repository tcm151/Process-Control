using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ProcessControl.Tools
{
    public static class UnityExtensions
    {
        //> LISTS ==================================================================================
        //- Remove and return the stack at the index position
        public static T TakeAndRemove<T>(this List<T> list, int index)
        {
            var item = list[index];
            list.RemoveAt(index);
            return item;
        }
        //------------------------------------------------------------------------------------------
        public static List<T> TakeAndRemoveRange<T>(this List<T> list, int start, int end)
        {
            var items = new List<T>();
            for (int i = start; i <= end; i++) items.Add(list[i]);
            return items;
        }
        //------------------------------------------------------------------------------------------
        //- Remove and return the first stack in the list
        public static T TakeAndRemoveFirst<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }
        //------------------------------------------------------------------------------------------
        //- Remove and return the last stack in the list
        public static T TakeAndRemoveLast<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            var item = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return item;
        }
        //------------------------------------------------------------------------------------------
        //- Return the stack after the stack given, returns first stack if last stack is given
        public static T ItemAfter<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);
            index = (index >= list.Count - 1) ? 0 : ++index;
            return list[index];
        }
        //------------------------------------------------------------------------------------------
        //- Shuffle a list, optional seed
        public static List<T> Shuffle<T>(this List<T> list, int seed = 69)
        {
            var random = new System.Random(seed);
            var n = list.Count;
            while (n > 1)
            {
                int k = random.Next(n--);
                (list[n], list[k]) = (list[k], list[n]);
            }
            return list;
        }
        //------------------------------------------------------------------------------------------
        public static T NextAfter<T>(this List<T> list, T currentItem)
        {
            var index = list.IndexOf(currentItem);
            return (index < list.Count - 1) ? list[index + 1] : default;
        }
        //------------------------------------------------------------------------------------------
        //- Combined ForEach with Where clause
        public static void ForEachWhere<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Action<T> action)
        {
            enumerable.ToList().ForEach(
                item =>
                {
                    if (predicate(item)) action(item);
                });
        }
        //------------------------------------------------------------------------------------------
        
        
        //> ARRAYS =================================================================================
        //- Convert a 2D array into a list
        public static List<T> ToList<T>(this T[,] array)
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
        //------------------------------------------------------------------------------------------
        //- Modify every stack in an array by the given action
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            for (int i = 0; i < array.Length; i++) action(array[i]);
        }
        //------------------------------------------------------------------------------------------
        //- Modify every stack in a 2d array by the given action
        public static void ForEach<T>(this T[,] array2d, Action<T> action)
        {
            for (int i = 0; i < array2d.GetLength(0); i++) {
                for (int j = 0; j < array2d.GetLength(1); j++)
                {
                    action(array2d[i, j]);
                }
            }
        }
        
        //------------------------------------------------------------------------------------------
        public static List<T> Where<T>(this T[,] array2d, Predicate<T> predicate)
        {
            var matches = new List<T>();
            array2d.ForEach(item => {if (predicate(item)) matches.Add(item);});
            return matches;
        }
        //------------------------------------------------------------------------------------------
        public static List<R> SelectMany<T, R>(this T[,] array2d, Func<T, R[,]> selector)
        {
            var listOfSelections = new List<R>();
            array2d.ForEach(item =>
            {
                selector(item).ForEach(selection =>
                {
                    listOfSelections.Add(selection);
                });
            });
            return listOfSelections;
        }
        //------------------------------------------------------------------------------------------
        
        
        //> CAMERA =================================================================================
        //- The mouse position in world space with z = 0
        public static Vector2 MousePosition2D(this Camera camera)
            => MousePosition3D(camera).ToVector2();
        //------------------------------------------------------------------------------------------
        //- The mouse position in world space
        public static Vector3 MousePosition3D(this Camera camera)
            => camera.ScreenToWorldPoint(Input.mousePosition);
        //------------------------------------------------------------------------------------------
        
        
        //> LAYER MASK =============================================================================
        //- does the given layer mask contain the given layer
        public static bool Contains(this LayerMask layerMask, int layer)
            => (layerMask == (layerMask | (1 << layer)));
        //------------------------------------------------------------------------------------------
        
        
        //> VECTOR2 ================================================================================
        //- Return the angle represented by the normalized vector2 direction
        public static float Angle(this Vector2 direction)
        {
            direction.Normalize();

            var angle = Mathf.Acos(direction.x) * Mathf.Rad2Deg;
            return (direction.y > 0f) ? angle : -angle;
        }
        //------------------------------------------------------------------------------------------
        //- Move a vector2 towards another vector2 by a maximum delta [modified by reference]
        public static Vector2 MoveTowardsR(this ref Vector2 current, Vector2 target, float maxDelta)
            => current = Vector2.MoveTowards(current, target, maxDelta);
        //------------------------------------------------------------------------------------------
        //- Lerp a vector2 towards another vector2 by a maximum delta [modified by reference]
        public static Vector2 LerpR(this ref Vector2 current, Vector2 target, float maxDelta)
            => current = Vector2.Lerp(current, target, maxDelta);
        //------------------------------------------------------------------------------------------
        //- Convert a vector2 into a vector2int
        public static Vector2Int FloorToInt(this Vector2 v2) => new Vector2Int
        {
            x = Mathf.FloorToInt(v2.x),
            y = Mathf.FloorToInt(v2.y),
        };
        //------------------------------------------------------------------------------------------
        //- Convert a vector2 into a vector3
        public static Vector3 ToVector3(this Vector2Int v2) => new Vector3
        {
            x = v2.x,
            y = v2.y,
            z = 0f,
        };
        //------------------------------------------------------------------------------------------
        public static Vector2 ToVector2(this Vector2Int v2) => new Vector2
        {
            x = v2.x,
            y = v2.y,
        };
        //------------------------------------------------------------------------------------------
        public static Vector3 ToVector3(this Vector2 v2) => new Vector3
        {
            x = v2.x,
            y = v2.y,
            z = 0f,
        };
        //------------------------------------------------------------------------------------------
        public static Vector2 RemapR(this ref Vector2 v2, float lower1, float upper1, float lower2, float upper2)
        {
            v2.x.Remap(lower1, upper1, lower2, upper2);
            v2.y.Remap(lower1, upper1, lower2, upper2);
            return v2;
        }
        //------------------------------------------------------------------------------------------
        public static Vector2 ClampMagnitudeR(this ref Vector2 vector2, float maxLength)
            => vector2 = Vector2.ClampMagnitude(vector2, maxLength);
        //------------------------------------------------------------------------------------------
        public static Vector2 AbsR(this ref Vector2 v2)
        {
            v2.x.Abs();
            v2.y.Abs();
            return v2;
        }
        //------------------------------------------------------------------------------------------


        //> VECTOR3 ================================================================================
        //- Return the angle represented by the normalized vector2 direction
        public static float Angle(this Vector3 direction)
        {
            direction.Normalize();

            var angle = Mathf.Acos(direction.x) * Mathf.Rad2Deg;
            return (direction.y > 0f) ? angle : -angle;
        }
        //------------------------------------------------------------------------------------------
        //- Move a vector3 towards another vector3 by a maximum delta [modified by reference]
        public static Vector3 MoveTowardsR(this ref Vector3 current, Vector3 target, float maxDelta)
            => current = Vector3.MoveTowards(current, target, maxDelta);
        //------------------------------------------------------------------------------------------
        //- Calculate the vector from the first vector to the second vector
        public static Vector3 VectorTo(this Vector3 first, Vector3 second)
            => second - first;
        //------------------------------------------------------------------------------------------
        //- Return the distance from the first vector to the second vector
        public static float DistanceTo(this Vector3 first, Vector3 second)
            => Vector3.Distance(first, second);
        //------------------------------------------------------------------------------------------
        //- Return the absolute value of the vector
        public static Vector3 Abs(this  Vector3 vector) => new Vector3
        {
            x = Mathf.Abs(vector.x),
            y = Mathf.Abs(vector.y),
            z = Mathf.Abs(vector.z),
        };
        //------------------------------------------------------------------------------------------
        //- Return the upper vector3int
        public static Vector3Int CeilToInt(this Vector3 vector) => new Vector3Int
        {
            x = Mathf.CeilToInt(vector.x),
            y = Mathf.CeilToInt(vector.y),
            z = Mathf.CeilToInt(vector.z),
        };
        //------------------------------------------------------------------------------------------
        //- Return the lower vector3int
        public static Vector3Int FloorToInt(this Vector3 vector) => new Vector3Int
        {
            x = Mathf.FloorToInt(vector.x),
            y = Mathf.FloorToInt(vector.y),
            z = Mathf.FloorToInt(vector.z),
            
        };
        //------------------------------------------------------------------------------------------
        //- Convert a vector3 into a vector2 [using the x & y coordinates]
        public static Vector2 ToVector2(this Vector3 vector3) => new Vector2
        {
            x = vector3.x,
            y = vector3.y,
        };
        //------------------------------------------------------------------------------------------
        //- Convert a vector3 into a vector2 [using the x & z coordinates]
        public static Vector2 ToVector2XZ(this Vector3 vector3) => new Vector2
        {
            x = vector3.x,
            y = vector3.z,
        };
        //------------------------------------------------------------------------------------------
        //- Calculate the direction from an origin vector to a target vector
        public static Vector3 DirectionTo(this Vector3 origin, Vector3 target)
            => (target - origin).normalized;
        //------------------------------------------------------------------------------------------
        
        
        //> MATHF ==================================================================================
        public static float Clamp(this float value, float min, float max)
            => Mathf.Clamp(value, min, max);
        //------------------------------------------------------------------------------------------
        public static float ClampR(this ref float value, float min, float max)
            => value = Mathf.Clamp(value, min, max);
        //------------------------------------------------------------------------------------------
        public static float MoveTowards(this float value, float target, float maxDela)
            => Mathf.MoveTowards(value, target, maxDela);
        //------------------------------------------------------------------------------------------
        public static float MoveTowardsR(this ref float value, float target, float maxDelta)
            => value = Mathf.MoveTowards(value, target, maxDelta);
        //------------------------------------------------------------------------------------------
        public static int FloorToInt(this float value)
            => Mathf.FloorToInt(value);
        //------------------------------------------------------------------------------------------
        public static int CeilToInt(this float value)
            => Mathf.CeilToInt(value);
        //------------------------------------------------------------------------------------------
        public static float Remap(this float value, float lower1, float upper1, float lower2, float upper2)
            => (value - lower1) / (upper1 - lower1) * (upper2 - lower2) + lower2;
        //------------------------------------------------------------------------------------------
        public static float RemapR(this ref float value, float lower1, float upper1, float lower2, float upper2)
            => value = (value - lower1) / (upper1 - lower1) * (upper2 - lower2) + lower2;
        //------------------------------------------------------------------------------------------
        public static float Abs(this ref float value)
            => value = Mathf.Abs(value);
        //------------------------------------------------------------------------------------------
        public static float Sqrt(this ref float value)
            => value = Mathf.Sqrt(value);
        //------------------------------------------------------------------------------------------
        public static float Lerp(this float value, float final, float delta)
            => value = Mathf.Lerp(value, final, delta);
        //------------------------------------------------------------------------------------------
    }
}