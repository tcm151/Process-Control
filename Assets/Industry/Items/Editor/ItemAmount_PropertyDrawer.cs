using UnityEditor;
using UnityEngine;


namespace ProcessControl.Industry.Items.Editor
{
    [CustomPropertyDrawer(typeof(Stack))]
    public class ItemAmount_PropertyDrawer : PropertyDrawer
    {
        private SerializedProperty item, amount;
        
        override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            item = property.FindPropertyRelative("item");
            amount = property.FindPropertyRelative("amount");
            
            EditorGUI.BeginProperty(rect, label, property);

            // var centerStyle = new GUIStyle
            // {
            //     alignment = TextAnchor.MiddleCenter,
            // };

            var amountRect = new Rect(rect.x, rect.y, rect.width * (1f / 6f), rect.height);
            EditorGUI.PropertyField(amountRect, amount, GUIContent.none);
            // EditorGUI.IntField(amountRect, GUIContent.none, amount.intValue);
            
            var itemRect = new Rect(rect.x + rect.width * (1f/6f), rect.y, rect.width * (5f / 6f), rect.height);
            EditorGUI.PropertyField(itemRect, item, GUIContent.none);
            
            EditorGUI.EndProperty();
        }

        override public float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}