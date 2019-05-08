using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS.ScriptableObjects
{
    //The MusicSequence class includes instructions for the music sequencer
    [System.Serializable, CreateAssetMenu(fileName = "RTH_", menuName = "MusicGenerator/Rhythm", order = 1)]
    public class Rhythm : ScriptableObject
    {
        public enum RhythmMode
        {
            Fixed,
            Manual
        };

        public RhythmMode rhythmMode = RhythmMode.Fixed;
    }
}