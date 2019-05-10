using GMS.ScriptableObjects;

namespace GMS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class SequenceGenerator
    {
        public static Note[] GenerateSequence(int pNoteCount, Scale pScale, SequenceData pSequenceData)
        {
            SequenceData.SequenceMode sequenceMode = pSequenceData.sequenceMode;
            switch (sequenceMode)
            {
                case SequenceData.SequenceMode.Legacy:
                    return GenerateLegacy(pNoteCount, pSequenceData);
                case SequenceData.SequenceMode.Simple:
                    return GenerateSimple(pNoteCount, pScale, pSequenceData);
            }

            return null;
        }

        public static Note[] GenerateLegacy(int pNoteCount, SequenceData pSequenceData)
        {
            Note[] note = new Note[pNoteCount];

            for (int i = 0; i < note.Length; i++)
            {
                note[i] = new Note(Note.Modes.Single, Random.Range(0.5f, 2.0f));
            }

            return note;
        }

        public static Note[] GenerateSimple(int pNoteCount, Scale pScale, SequenceData pSequenceData)
        {
            Note[] note = new Note[pNoteCount];
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