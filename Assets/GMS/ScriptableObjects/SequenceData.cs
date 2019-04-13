using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    //The MusicSequence class includes instructions for the music sequencer
    [System.Serializable, CreateAssetMenu(fileName = "SEQ_", menuName = "MusicGenerator/Sequence Data", order = 1)]
    public class SequenceData : ScriptableObject
    {
        public enum SequenceMode
        {
            Solo,
            Chords,
            Legacy
        };

        public SequenceMode sequenceMode = SequenceMode.Legacy;

        [SerializeField] public MGSound sound;

        [SerializeField] public Note[] notes = new Note[16];

        bool quantize;

        bool
            globalPitchVar; //Chord mode: Indicates whether the same pitch variation is applied to all notes or per note
    }
}