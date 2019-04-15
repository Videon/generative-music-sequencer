using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS.ScriptableObjects
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

        public int dontRepeatLast;    //Amount of last played notes that are not considered for the generation of a new sequence

        //public bool quantize;    //Indicates whether notes are only generated on steps
        public bool useGlobalScale; //Indicates whether the global bar scale is used for generating a new sequence

        public bool
            globalPitchVar; //Chord mode: Indicates whether the same pitch variation is applied to all notes or per note
    }
}