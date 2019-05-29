using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GMS.ScriptableObjects
{
    [System.Serializable, CreateAssetMenu(fileName = "SST_", menuName = "MusicGenerator/Sequencer State", order = 1)]
    public class SequencerState : ScriptableObject
    {
        [SerializeField] public double bpm;
        [SerializeField] public int barSteps;

        [SerializeField] public SequenceData[] musicSequences;
        [SerializeField] public Rhythm[] rhythms;
        [SerializeField] public Scale[] scales;
    }
}