using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace ProcessControl.Tools.Editor
{
    [CustomPropertyDrawer(typeof(Range))]
    public class Range_PropertyDrawer : PropertyDrawer
    {
        private SerializedProperty min, max;
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            min = property.FindPropertyRelative("min");
            max = property.FindPropertyRelative("max");

            EditorGUI.BeginProperty(rect, label, property);

            var minMaxRect = EditorGUI.PrefixLabel(rect, label);
            
            var minRect = new Rect(minMaxRect.x, minMaxRect.y, minMaxRect.width / 2, minMaxRect.height);
            EditorGUI.PropertyField(minRect, min, GUIContent.none);
            
            var maxRect = new Rect(minMaxRect.x + minMaxRect.width / 2, minMaxRect.y, minMaxRect.width / 2, minMaxRect.height);
            EditorGUI.PropertyField(maxRect, max, GUIContent.none);
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}