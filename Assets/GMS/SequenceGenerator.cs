using System;
using GMS.ScriptableObjects;

namespace GMS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class SequenceGenerator
    {
        public static Note[] GenerateSequence(double pBpm, int pBarSteps, double[] pGeneratedRhythm, Scale pScale,
            SequenceData pSequenceData)
        {
            SequenceData.GeneratorMode generatorMode = pSequenceData.generatorMode;
            switch (generatorMode)
            {
                case SequenceData.GeneratorMode.Legacy:
                    return GenerateLegacy(pGeneratedRhythm.Length, pSequenceData);
                case SequenceData.GeneratorMode.Simple:
                    return GenerateSimple(pGeneratedRhythm.Length, pScale, pSequenceData);
                case SequenceData.GeneratorMode.WeightedScale:
                    return GenerateWeightedScale(pGeneratedRhythm.Length, pScale, pSequenceData);
                case SequenceData.GeneratorMode.Curve:
                    return GenerateCurve(pBpm, pBarSteps, pGeneratedRhythm, pScale, pSequenceData);
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

        #region Curve Generation Mode

        public static Note[] GenerateCurve(double pBpm, int pBarSteps, double[] pGeneratedRhythm, Scale pScale,
            SequenceData pSequenceData)
        {
            Note[] note = new Note[pGeneratedRhythm.Length];

            if (pSequenceData.useExternalParams)
                UpdateCurve(pSequenceData);


            for (int i = 0; i < note.Length; i++)
            {
                note[i] = new Note(Note.Modes.Single,
                    TimeToPitch(pBpm, pBarSteps, pGeneratedRhythm[i], pScale, pSequenceData));
            }

            return note;
        }

        /// <summary>Update local curve of sequence when using external inputs.</summary>
        private static void UpdateCurve(SequenceData pSequenceData)
        {
            Keyframe[] keyframes = new Keyframe[pSequenceData.inputs.Length];
            for (int i = 1; i <= keyframes.Length; i++)
            {
                keyframes[i - 1].time = (1.0f / keyframes.Length) * i;
                keyframes[i - 1].value = pSequenceData.inputs[i - 1].paramVal;
            }

            pSequenceData.curve.keys = keyframes;
        }


        private static float TimeToPitch(double pBpm, int pBarSteps, double noteTime, Scale pScale,
            SequenceData pSequenceData)
        {
            List<int> enabledNotes = new List<int>();
            //This is adding all enabled notes from the list, sorted from lowest to highest
            for (int i = 0; i < pScale.scaleActiveNotes.Length; i++)
                if (pScale.scaleActiveNotes[i])
                    enabledNotes.Add(i);


            float curveValue =
                Mathf.Clamp(pSequenceData.curve.Evaluate((float) GetNormalizedTime(pBpm, pBarSteps, noteTime)),
                    0.0f, 1.0f);
            int currentNote = (enabledNotes[Mathf.RoundToInt((enabledNotes.Count - 1) * curveValue)]);
            return pScale.CalculateFrequency(currentNote);
        }

        /// <summary> Normalizes absolute note time to relative note time (value range 0 to 1) </summary>
        /// <param name="pBpm"></param>
        /// <param name="pBarSteps"></param>
        /// <param name="noteTime"></param>
        /// <returns></returns>
        private static double GetNormalizedTime(double pBpm, int pBarSteps, double noteTime)
        {
            double barLength = (60d / pBpm) * pBarSteps;
            return noteTime / barLength;
        }

        #endregion


        private static Note[] GenerateSimple(int pNoteCount, Scale pScale, SequenceData pSequenceData)
        {
            Note[] note = new Note[pNoteCount];
            List<int> enabledNotes = new List<int>();

            //This is adding all enabled notes from the list, sorted from lowest to highest
            for (int i = 0; i < pScale.scaleActiveNotes.Length; i++)
                if (pScale.scaleActiveNotes[i])
                    enabledNotes.Add(i);

            for (int i = 0; i < note.Length; i++)
            {
                note[i] = new Note(Note.Modes.Single,
                    pScale.CalculateFrequency(enabledNotes[Random.Range(0, enabledNotes.Count)]));
            }

            return note;
        }


        #region Weighted Scale Generation Mode

        private static Note[] GenerateWeightedScale(int pNoteCount, Scale pScale, SequenceData pSequenceData)
        {
            Note[] note = new Note[pNoteCount];
            List<int> enabledNotes = new List<int>();


            //This is adding all enabled notes from the list, sorted from lowest to highest
            for (int i = 0; i < pScale.scaleActiveNotes.Length; i++)
                if (pScale.scaleActiveNotes[i] == true)
                    enabledNotes.Add(i);


            LinkedParameter input;
            if (pSequenceData.inputs[0] != null)
                input = pSequenceData.inputs[0];
            else
            {
                input = null;
            }

            float[] noteWeights = CalcNoteWeights(enabledNotes.ToArray(), input);

            for (int i = 0; i < note.Length; i++)
            {
                note[i] = new Note(Note.Modes.Single,
                    pScale.CalculateFrequency(enabledNotes[CalcWeightedRandomVal(noteWeights)]));
            }

            return note;
        }

        private static float[] CalcNoteWeights(int[] pEnabledNotes, LinkedParameter input)
        {
            float[] noteWeights = new float[pEnabledNotes.Length]; //Array for storing note weights
            int weightCenter = Mathf.RoundToInt(input.paramVal * pEnabledNotes.Length);

            float width = 10.0f;

            for (int i = 0; i < noteWeights.Length; i++)
            {
                noteWeights[i] = (-Mathf.Pow(i - weightCenter, 2.0f) / width) + 1.0f;
                if (noteWeights[i] < 0)
                    noteWeights[i] = 0;
            }

            return noteWeights;
        }

        /// <summary>Returns a weighted random position based on passed weights.</summary>
        /// <param name="pNoteWeights"></param>
        /// <returns></returns>
        private static int CalcWeightedRandomVal(float[] pNoteWeights)
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

        #endregion
    }
}