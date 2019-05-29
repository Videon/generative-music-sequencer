using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS.ScriptableObjects
{
    //The MusicSequence class includes instructions for the music sequencer
    [System.Serializable, CreateAssetMenu(fileName = "RTH_", menuName = "MusicGenerator/Rhythm", order = 1)]
    public class Rhythm : ScriptableObject
    {
        [SerializeField] public int steps = 16;
        [SerializeField] public int divider = 4;

        public enum RhythmMode
        {
            AutomaticFixed,
            Manual
        };

        public RhythmMode rhythmMode = RhythmMode.AutomaticFixed;
    }
}