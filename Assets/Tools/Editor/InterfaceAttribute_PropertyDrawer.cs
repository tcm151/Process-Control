using UnityEngine;
using UnityEditor;

namespace ProcessControl.Tools.Editor
{
    #if UNITY_EDITOR
    
    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    public class RequireInterfaceDrawer : PropertyDrawer
    {
        override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var requiredAttribute = this.attribute as RequireInterfaceAttribute;
                EditorGUI.BeginProperty(position, label, property);
                var reference = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Object), true);
                if (reference is null)
                {
                    var obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Object), true);
                    if (obj is GameObject gameObject)
                    {
                        property.objectReferenceValue = gameObject.GetComponent(requiredAttribute?.requiredType);
                    }
                }
                property.objectReferenceValue = reference;
                EditorGUI.EndProperty();
            }
            else
            {
                var previousColor = GUI.color;
                GUI.color = Color.red;
                EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));
                GUI.color = previousColor;
            }
        }
    }
    
    #endif
}

