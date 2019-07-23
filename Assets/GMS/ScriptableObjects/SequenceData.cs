using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS.ScriptableObjects
{
    //The MusicSequence class includes instructions for the music sequencer
    [System.Serializable, CreateAssetMenu(fileName = "SEQ_", menuName = "MusicGenerator/Sequence Data", order = 1)]
    public class SequenceData : ScriptableObject
    {
        public enum GeneratorMode
        {
            WeightedScale,
            Curve,
            Chords,
            Legacy,
            Simple
        };

        public GeneratorMode generatorMode = GeneratorMode.Legacy;

        [SerializeField] public MGSound sound;

        [SerializeField] public Note[] notes;

        [SerializeField]
        public bool
            useExternalParams; //Indicates whether external parameters (Linkedparameter input) will be considered for sequence generation

        [SerializeField] public LinkedParameter[] inputs;

        [SerializeField] public AnimationCurve curve;

        //public int dontRepeatLast; //Amount of last played notes that are not considered for the generation of a new sequence

        //public bool quantize;     //Indicates whether notes are only generated on steps
        public Scale localScale; //If not empty, custom scale will be used for sequence generation
        public Rhythm localRhythm;

        //public bool globalPitchVar; //Chord mode: Indicates whether the same pitch variation is applied to all notes or per note
    }
}