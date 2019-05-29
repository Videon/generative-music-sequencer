using System;
using GMS.ScriptableObjects;

namespace GMS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class RhythmGenerator
    {
        public static double[] GenerateRhythm(double pBpm, int pBarSteps, Rhythm pRhythm)
        {
            Rhythm.RhythmMode rhythmMode = pRhythm.rhythmMode;
            switch (rhythmMode)
            {
                case Rhythm.RhythmMode.AutomaticFixed:
                    return GenerateAutomaticFixed(pBpm, pBarSteps, pRhythm);
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
        private static double[] GenerateAutomaticFixed(double pBpm, int pBarSteps, Rhythm pRhythm)
        {
            double[] rhythmOutput = new double[pRhythm.steps];
            double stepLength = ((60d / pBpm) * pBarSteps) / (rhythmOutput.Length);
            for (int i = 0; i < rhythmOutput.Length; i++)
                rhythmOutput[i] = i * stepLength;
            return rhythmOutput;
        }
    }
}