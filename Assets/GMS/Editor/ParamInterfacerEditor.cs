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

        paramInterfacer.InitParameters(EditorGUILayout.IntField("Parameter count", paramInterfacer.parameters.Count));

        for (int i = 0; i < paramInterfacer.parameters.Count; i++)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            paramInterfacer.parameters[i].paramName =
                EditorGUILayout.TextField("Parameter Name", paramInterfacer.parameters[i].paramName);
            EditorGUI.BeginChangeCheck();
            paramInterfacer.parameters[i].inputVal =
                EditorGUILayout.FloatField("Input Value", paramInterfacer.parameters[i].inputVal);


            EditorGUILayout.LabelField("Normalization parameters");
            paramInterfacer.parameters[i].minNormal =
                EditorGUILayout.FloatField("min", paramInterfacer.parameters[i].minNormal);
            paramInterfacer.parameters[i].maxNormal =
                EditorGUILayout.FloatField("max", paramInterfacer.parameters[i].maxNormal);

            //Trigger recalculation of normalized output value 
            if (EditorGUI.EndChangeCheck())
            {
                paramInterfacer.SetParamValue(paramInterfacer.parameters[i].paramName,
                    paramInterfacer.parameters[i].inputVal);
            }

            EditorGUILayout.LabelField("Normalized Output Value: " + paramInterfacer.parameters[i].outputVal);

            EditorGUILayout.EndVertical();
        }
    }
}