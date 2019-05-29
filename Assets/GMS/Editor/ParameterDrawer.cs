using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParameterDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) * 2;
    }
}