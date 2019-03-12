using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicSequencer))]
public class MusicSequencerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        if (GUILayout.Button("Open Sequencer Window"))
        {
            EditorWindow sequencerWindow = EditorWindow.GetWindow(typeof(SequencerWindow), false, "Sequencer");
        }
    }
}
