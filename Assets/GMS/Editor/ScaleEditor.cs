﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMS.ScriptableObjects;
using UnityEditor;

[CustomEditor(typeof(Scale))]
public class ScaleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Scale scale = (Scale) target;
        bool[] activeNotes = scale.scaleActiveNotes;

        EditorGUILayout.BeginHorizontal();
        int row = 0;
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < activeNotes.Length; i++)
        {
            if (row != i / 12)
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                row = i / 12;
            }

            activeNotes[i] =
                EditorGUILayout.ToggleLeft((NoteToName(i) + (int) (i / 12)), activeNotes[i], GUILayout.Width(50f));
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        scale.scaleActiveNotes = activeNotes;
    }

    private string NoteToName(int noteIndex)
    {
        int checkVal = noteIndex % 12;
        switch (checkVal)
        {
            case 0:
                return "C";
            case 1:
                return "C#";
            case 2:
                return "D";
            case 3:
                return "D#";
            case 4:
                return "E";
            case 5:
                return "F";
            case 6:
                return "F#";
            case 7:
                return "G";
            case 8:
                return "G#";
            case 9:
                return "A";
            case 10:
                return "A#";
            case 11:
                return "H";
        }

        return "--";
    }
}