using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SND_", menuName = "MusicGenerator/Sound", order = 1)]
    public class MGSound : ScriptableObject
    {
        [SerializeField, Tooltip("Maximum number of notes that can be played simultaneously")]
        public int polyphony = 1;

        [SerializeField] public string fileName; //The filename of the assigned sound
        [SerializeField] public float gain;

        [SerializeField]
        bool multisample; //Indicates whether sound consists of one sample that is pitched or one sample per note.

        [SerializeField] float pitchOffset; //Fixed offset value for sound pitch

        [SerializeField] float minPitchVar, maxPitchVar; //Minimum and maximum variation in pitch on note play
    }
}