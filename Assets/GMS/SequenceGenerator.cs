using System;
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
            SequenceData.GeneratorMode generatorMode = pSequenceData.generatorMode;
            switch (generatorMode)
            {
                case SequenceData.GeneratorMode.Legacy:
                    return GenerateLegacy(pNoteCount, pSequenceData);
                case SequenceData.GeneratorMode.Simple:
                    return GenerateSimple(pNoteCount, pScale, pSequenceData);
                case SequenceData.GeneratorMode.WeightedScale:
                    return GenerateWeightedScale(pNoteCount, pScale, pSequenceData);
                case SequenceData.GeneratorMode.Chords:
                    throw new NotImplementedException();
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

            //This is adding all enabled notes from the list, sorted from lowest to highest
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

        public static Note[] GenerateWeightedScale(int pNoteCount, Scale pScale, SequenceData pSequenceData)
        {
            Note[] note = new Note[pNoteCount];
            List<int> enabledNotes = new List<int>();


            //This is adding all enabled notes from the list, sorted from lowest to highest
            for (int i = 0; i < pScale.scaleActiveNotes.Length; i++)
                if (pScale.scaleActiveNotes[i] == true)
                    enabledNotes.Add(i);


            float[] noteWeights = CalcNoteWeights(enabledNotes.ToArray());

            for (int i = 0; i < note.Length; i++)
            {
                note[i] = new Note(Note.Modes.Single,
                    pScale.CalculateFrequency(enabledNotes[CalcWeightedRandomVal(noteWeights)]));
            }

            return note;
        }

        public static float[] CalcNoteWeights(int[] pEnabledNotes)
        {
            float[] noteWeights = new float[pEnabledNotes.Length]; //Array for storing note weights

            float input = 0.5f; //todo change into globally accessible parameter
            int weightCenter = Mathf.RoundToInt(input * pEnabledNotes.Length);

            float width = 10.0f;

            for (int i = 0; i < noteWeights.Length; i++)
            {
                noteWeights[i] = (-Mathf.Pow(i - weightCenter, 2.0f) / width) + 1.0f;
                if (noteWeights[i] < 0)
                    noteWeights[i] = 0;
            }

            return noteWeights;
        }

        public static int CalcWeightedRandomVal(float[] pNoteWeights)
        {
            float total = 0;
            for (int i = 0; i < pNoteWeights.Length; i++)
            {
                total += pNoteWeights[i];
            }

            float rand = Random.Range(0, total);

            for (int i = 0; i < pNoteWeights.Length; i++)
            {
                if (rand < pNoteWeights[i])
                    return i;
            }

            return 0;
        }
    }
}