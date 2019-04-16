using GMS.ScriptableObjects;

namespace GMS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SequenceGenerator : MonoBehaviour
    {
        public static Note[] GenerateLegacy(SequenceData pSequenceData, int pBarSteps)
        {
            Note[] note = new Note[pBarSteps];

            for (int i = 0; i < note.Length; i++)
            {
                note[i] = new Note(Note.Modes.Single, Random.Range(0.5f, 2.0f));
            }

            return note;
        }

        public static Note[] GenerateSimple(SequenceData pSequenceData, Scale pScale, int pBarSteps)
        {
            Note[] note = new Note[pBarSteps];
            List<int> enabledNotes = new List<int>();
            for (int i = 0; i < pScale.scaleActiveNotes.Length; i++)
                if (pScale.scaleActiveNotes[i] == true)
                    enabledNotes.Add(i);

            for (int i = 0; i < note.Length; i++)
            {
                note[i] = new Note(Note.Modes.Single,
                    pScale.CalculateFrequency(enabledNotes[Random.Range(0, enabledNotes.Count)]));
            }

            return note;
        }
    }
}