namespace GMS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    //The MusicSequence class includes instructions for the music sequencer
    [System.Serializable, CreateAssetMenu(fileName = "SEQ_", menuName = "MusicGenerator/Sequence Data", order = 1)]
    public class SequenceData : ScriptableObject
    {
        public enum SequenceMode { Solo, Chords, Legacy };
        public SequenceMode sequenceMode = SequenceMode.Legacy;

        [SerializeField]
        public MGSound sound;

        [SerializeField]
        public Note[] notes = new Note[16];

        bool quantize;

        bool globalPitchVar;    //Chord mode: Indicates whether the same pitch variation is applied to all notes or per note
    }

    public class Note
    {
        public enum Modes { Single, Chord, Rhythm };
        public enum Length { Whole, Half, Quarter, Eighth, Sixteenth };

        public Modes mode;
        public Length length;
        public float pitch;

        public Note[] chordNotes;

        public Note(Modes p_mode, Length p_length, float p_pitch)
        {
            mode = p_mode;
            //length = p_length;
            pitch = p_pitch;
        }

        public Note(Modes p_mode, float p_pitch)
        {
            mode = p_mode;
            pitch = p_pitch;
        }
    }
}