using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using GMS;
using GMS.ScriptableObjects;
using Object = UnityEngine.Object;

[System.Serializable]
class SequencerWindow : EditorWindow
{
    [SerializeField] private MusicSequencer musicSequencer;

    Object source;

    //GUIStyle _style = new GUIStyle(EditorStyles.label);

    int bars = 1;
    int layers = 1;

    private Editor _itemEditor;

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

                RenderWindowSequencer();
            }
            else
                EditorGUILayout.LabelField("Select Sequencer first");
        }
        else
            EditorGUILayout.LabelField("Select Sequencer first");
    }

    void OnGUI()
    {
        if (musicSequencer != null)
        {
            RenderWindowGeneralSettings();
            RenderWindowSequencer();
            RenderWindowItemInspector();
        }
    }

    ///<summary>Function to update the displayed values in this editor script.</summary>
    void UpdateValues()
    {
    }

    void RenderWindowGeneralSettings()
    {
        EditorGUILayout.BeginVertical();
        bars = EditorGUILayout.IntField("Bars", bars);
        layers = EditorGUILayout.IntField("Layers", layers);

        if (GUILayout.Button("Set Sequencer"))
        {
            musicSequencer.InitSequencer(bars, layers);
        }

        EditorGUILayout.BeginHorizontal();
        musicSequencer.bpm = EditorGUILayout.DoubleField("BPM", musicSequencer.bpm);
        musicSequencer.barSteps = EditorGUILayout.IntField("Steps per bar", musicSequencer.barSteps);
        EditorGUILayout.EndHorizontal();

        //Realtime song position info
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Bar: " + (musicSequencer.GetCurrentBar() + 1), GUILayout.Width(100f));
        EditorGUILayout.LabelField("Current Step: " + (musicSequencer.currentStep + 1), GUILayout.Width(100f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.EndVertical();
    }

    ///<summary>Function to display the values in this editor script window.</summary>
    void RenderWindowSequencer()
    {
        GUI.SetNextControlName("");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUILayout.Width(50f)); //Dummy Label to offset table
        EditorGUILayout.LabelField("Rhythm", GUILayout.Width(50f)); //Rhythm label
        EditorGUILayout.LabelField("Scales", GUILayout.Width(50f)); //Scale label
        for (int y = 0; y < layers; y++)
            EditorGUILayout.LabelField("Layer " + (y + 1), GUILayout.Width(50f));
        EditorGUILayout.EndVertical();

        for (int x = 0; x < musicSequencer.GetMusicSequencesDimensions().x; x++)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Bar " + (x + 1), GUILayout.Width(50f));

            //Draw rhythm object fields
            GUI.SetNextControlName(("RhythmField_" + x +
                                    ",0")); //",0" -> 0 is a dummy value to feed parameters. //todo check if overloading methods is an option.
            musicSequencer.SetRhythm(x,
                (Rhythm) EditorGUILayout.ObjectField(musicSequencer.GetRhythm(x), typeof(Rhythm), false));

            //Draw scale object fields
            GUI.SetNextControlName("ScaleField_" + x + ",0");
            musicSequencer.SetScale(x,
                (Scale) EditorGUILayout.ObjectField(musicSequencer.GetScale(x), typeof(Scale), false));

            //Draw sequence object fields
            for (int y = 0; y < musicSequencer.GetMusicSequencesDimensions().y; y++)
            {
                GUI.SetNextControlName("SequenceField_" + x + "," + y);
                musicSequencer.SetMusicSequence(x, y, (SequenceData) EditorGUILayout.ObjectField(
                    musicSequencer.GetMusicSequence(x, y),
                    typeof(SequenceData), false));
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.EndVertical();
    }

    ///<summary>Draws the content of the selected object in the sequencer.</summary>
    void RenderWindowItemInspector()
    {
        GUI.SetNextControlName("");
        string focusedControl = GUI.GetNameOfFocusedControl();
        //Debug.Log(GUI.GetNameOfFocusedControl());

        if (!string.IsNullOrEmpty(focusedControl))
        {
            int typeSeparatorIndex = GetStringSplitterCharIndex(focusedControl, '_');
            int coordSeparatorIndex = GetStringSplitterCharIndex(focusedControl, ',');


            int x = int.Parse(focusedControl.Substring(typeSeparatorIndex + 1,
                coordSeparatorIndex - (typeSeparatorIndex + 1)));
            int y = int.Parse(focusedControl.Substring(coordSeparatorIndex + 1,
                focusedControl.Length - (coordSeparatorIndex + 1)));

            string objectType = focusedControl.Substring(0, typeSeparatorIndex);
            switch (objectType)
            {
                case "RhythmField":
                    if (x > -1)
                    {
                        _itemEditor = Editor.CreateEditor(musicSequencer.GetRhythm(x));
                    }

                    break;

                case "ScaleField":
                    if (x > -1)
                    {
                        _itemEditor =
                            (ScaleEditor) Editor.CreateEditor(musicSequencer.GetScale(x), typeof(ScaleEditor));
                    }

                    break;
                case "SequenceField":
                    if (x > -1 && y > -1)
                    {
                        _itemEditor = Editor.CreateEditor(musicSequencer.GetMusicSequence(x, y));
                    }

                    break;
            }
        }

        if (_itemEditor != null)
        {
            _itemEditor.OnInspectorGUI();
        }
    }

    private int GetStringSplitterCharIndex(string pString, char pSplitterChar)
    {
        for (int i = 0; i < pString.Length; i++)
            if (pString[i].Equals(pSplitterChar))
            {
                return i;
            }

        return -1;
    }
}