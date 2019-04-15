using UnityEngine;
using UnityEditor;
using System.Collections;
using GMS;
using GMS.ScriptableObjects;

[System.Serializable]
class SequencerWindow : EditorWindow
{
    [SerializeField] private MusicSequencer musicSequencer;

    Object source;

    int bars = 1;
    int layers = 1;

    [MenuItem("Window/MusicGenerator/SequencerWindow")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SequencerWindow));
    }

    public void Awake()
    {
        GameObject selection = (GameObject) Selection.activeObject;

        if (selection.GetType() == typeof(GameObject))
        {
            selection = (GameObject) Selection.activeObject;

            if (selection.GetComponent<MusicSequencer>() != null)
            {
                musicSequencer = selection.GetComponent<MusicSequencer>();

                var serializedMusicSequencer = new SerializedObject(musicSequencer);
                serializedMusicSequencer.Update();

                if (musicSequencer.GetMusicSequencesDimensions().x > 0 &&
                    musicSequencer.GetMusicSequencesDimensions().y > 0)
                {
                    bars = musicSequencer.GetMusicSequencesDimensions().x;
                    layers = musicSequencer.GetMusicSequencesDimensions().y;
                }

                RenderWindow();
            }
            else
                EditorGUILayout.LabelField("Select Sequencer first");
        }
        else
            EditorGUILayout.LabelField("Select Sequencer first");
    }

    void OnGUI()
    {
        RenderWindow();
    }

    ///<summary>Function to update the displayed values in this editor script.</summary>
    void UpdateValues()
    {
    }

    ///<summary>Function to display the values in this editor script window.</summary>
    void RenderWindow()
    {
        if (musicSequencer != null)
        {
            bars = EditorGUILayout.IntField("Bars", bars);
            layers = EditorGUILayout.IntField("Layers", layers);

            if (GUILayout.Button("Set Sequencer"))
            {
                musicSequencer.InitSequences(bars, layers);
            }

            musicSequencer.bpm = EditorGUILayout.DoubleField("BPM", musicSequencer.bpm);
            musicSequencer.barSteps = EditorGUILayout.IntField("Steps per bar", musicSequencer.barSteps);

            DrawGrid();

            if (GUI.GetNameOfFocusedControl().StartsWith("SequenceField"))
            {
                Debug.Log("COOL"); //Insert code here to draw preview window for Sequence Element
            }
        }
    }

    void DrawGrid()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUILayout.Width(50f)); //Dummy Label to offset table
        for (int y = 0; y < layers; y++)
            EditorGUILayout.LabelField("Layer " + (y + 1), GUILayout.Width(50f));
        EditorGUILayout.EndVertical();

        for (int x = 0; x < musicSequencer.GetMusicSequencesDimensions().x; x++)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Bar " + (x + 1), GUILayout.Width(50f));
            for (int y = 0; y < musicSequencer.GetMusicSequencesDimensions().y; y++)
            {
                GUI.SetNextControlName("SequenceField_" + x + "," + y);
                musicSequencer.InitMusicSequences
                (x, y,
                    (SequenceData) EditorGUILayout.ObjectField(musicSequencer.GetMusicSequence(x, y),
                        typeof(SequenceData), true));
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
    }
}