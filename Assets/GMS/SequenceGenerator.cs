namespace GMS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SequenceGenerator : MonoBehaviour
    {
        public static Note[] GenerateLegacy(SequenceData p_sequenceData, int p_barLength)
        {
            Note[] note = new Note[p_barLength];

            for (int i = 0; i < note.Length; i++)
            {
                note[i] = new Note(Note.Modes.Single, Random.Range(0.5f, 2.0f));
            }
            return note;
        }
    }
}