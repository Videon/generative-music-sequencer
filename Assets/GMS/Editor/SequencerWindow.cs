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

    //GUIStyle _style = new GUIStyle(EditorStyles.label);

    int bars = 1;
    int layers = 1;

    [MenuItem("Window/MusicGenerator/SequencerWindow")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SequencerWindow));
    }

    private void OnInspectorUpdate()
    {
        Repaint();
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
                musicSequencer.InitSequencer(bars, layers);
            }

            musicSequencer.bpm = EditorGUILayout.DoubleField("BPM", musicSequencer.bpm);
            musicSequencer.barSteps = EditorGUILayout.IntField("Steps per bar", musicSequencer.barSteps);

            //Realtime song position info
            EditorGUILayout.LabelField("Current Step: " + musicSequencer.currentStep, GUILayout.Width(100f));
            EditorGUILayout.LabelField("Current Bar: " + musicSequencer.GetCurrentBar(), GUILayout.Width(100f));

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
        EditorGUILayout.LabelField("Scales", GUILayout.Width(50f)); //Scale label
        for (int y = 0; y < layers; y++)
            EditorGUILayout.LabelField("Layer " + (y + 1), GUILayout.Width(50f));
        EditorGUILayout.EndVertical();

        for (int x = 0; x < musicSequencer.GetMusicSequencesDimensions().x; x++)
        {
            EditorGUILayout.BeginVertical();
            /*TODO FIX BAR MARKER TEXT CHANGE AND EDITOR CRASH
             * //Set color of bar label to green if bar is currently active, else set to default black color
            if (musicSequencer.GetCurrentBar() == x)
                _style.normal.textColor = Color.green;
            else
                _style.normal.textColor = Color.black;
            */
            EditorGUILayout.LabelField("Bar " + (x + 1),
                GUILayout.Width(50f)); //todo don't forget to add back _style here when above fix is implemented
            musicSequencer.SetScale(x,
                (Scale) EditorGUILayout.ObjectField(musicSequencer.GetScale(x), typeof(Scale), false));
            for (int y = 0; y < musicSequencer.GetMusicSequencesDimensions().y; y++)
            {
                GUI.SetNextControlName("SequenceField_" + x + "," + y);
                musicSequencer.SetMusicSequence
                (x, y,
                    (SequenceData) EditorGUILayout.ObjectField(musicSequencer.GetMusicSequence(x, y),
                        typeof(SequenceData), false));
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
    }
}