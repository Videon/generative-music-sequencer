using System;
using GMS.ScriptableObjects;

namespace GMS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class RhythmGenerator
    {
        public static Note[] GenerateRhythm(int pBarSteps, Rhythm pRhythm)
        {
            Rhythm.RhythmMode rhythmMode = pRhythm.rhythmMode;
            switch (rhythmMode)
            {
                case Rhythm.RhythmMode.AutomaticFixed:
                    return GenerateAutomaticFixed(pBarSteps, pRhythm);
                case Rhythm.RhythmMode.RandomMinMax:
                    return GenerateRandomMinMax(pBarSteps, pRhythm);
                case Rhythm.RhythmMode.Manual:
                    throw new NotImplementedException();
            }

            return null;
        }

        /// <summary>Generates a fixed rhythm, based on the number of steps indicated in the rhythm object.</summary>
        /// <param name="pBpm">Current playback speed.</param>
        /// <param name="pBarSteps">Current global steps per bar.</param>
        /// <param name="pRhythm">Input generation parameters.</param>
        /// <returns></returns>
        private static Note[] GenerateAutomaticFixed(int pBarSteps, Rhythm pRhythm)
        {
            Note[] rhythmOutput = new Note[pRhythm.steps];
            double stepLength = 1.0d / (rhythmOutput.Length);
            for (int i = 0; i < rhythmOutput.Length; i++)
                rhythmOutput[i] = new Note(i * stepLength);
            return rhythmOutput;
        }

        private static Note[] GenerateRandomMinMax(int pBarSteps, Rhythm pRhythm)
        {
            List<Note> rhythmOutput = new List<Note>();
            double currentTime = 0.0d;
            while (currentTime < pBarSteps)
            {
                currentTime += Random.Range(pRhythm.randomMin, pRhythm.randomMax);
                if (currentTime >= pBarSteps)
                    break;

                rhythmOutput.Add(new Note(currentTime));
            }

            return rhythmOutput.ToArray();
        }
    }
}