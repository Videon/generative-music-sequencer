using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GMS;

[CustomEditor(typeof(ParamInterfacer))]
public class ParamInterfacerEditor : Editor
{
    private GUIStyle _guiStyle;

    void Start()
    {
        _guiStyle = new GUIStyle();
    }

    public override void OnInspectorGUI()
    {
        ParamInterfacer paramInterfacer = (ParamInterfacer) target;

        paramInterfacer.SetParameters(EditorGUILayout.IntField("Parameter count", paramInterfacer.parameters.Count));

        for (int i = 0; i < paramInterfacer.parameters.Count; i++)
        {
            EditorGUILayout.BeginVertical();
            paramInterfacer.parameters[i].paramName =
                EditorGUILayout.TextField("Parameter Name", paramInterfacer.parameters[i].paramName);
            paramInterfacer.parameters[i].inputVal =
                EditorGUILayout.FloatField("Input Value", paramInterfacer.parameters[i].inputVal);

            EditorGUILayout.LabelField("Normalization parameters");
            paramInterfacer.parameters[i].minNormal =
                EditorGUILayout.FloatField("min", paramInterfacer.parameters[i].minNormal);
            paramInterfacer.parameters[i].maxNormal =
                EditorGUILayout.FloatField("max", paramInterfacer.parameters[i].maxNormal);

            EditorGUILayout.LabelField("Normalized Output Value: " + paramInterfacer.parameters[i].outputVal);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.EndVertical();
        }
    }
}