using UnityEngine;

namespace GMS.ScriptableObjects
{
    /// <summary>
    /// Class to store the enabled notes in a set scale.
    /// </summary>
    [CreateAssetMenu(fileName = "SCL_", menuName = "MusicGenerator/Scale", order = 1)]
    public class Scale : ScriptableObject
    {
        public bool isWeighted;
        public bool[] scaleActiveNotes;
        public float[] noteWeight;

        //TODO Change calculation method to use double values in order to improve pitch precision
        private const float rootPitch = 0.625f;
        private float _pitchInterval = Mathf.Pow(2.0f, 1.0f / 12.0f);

        public Scale()
        {
            scaleActiveNotes = new bool[128];
        }

        /// <summary>
        /// Set individual values in scale.
        /// </summary>
        public void SetValues(int posX, bool pValue)
        {
            if (posX > 0 && posX < scaleActiveNotes.Length)
                scaleActiveNotes[posX] = pValue;
        }

        public float CalculateFrequency(int note)
        {
            //A4 = 440hz
            //Increment factor for 12 semi-tones: 12th root(2) per semitone
            //For calculating tone pitch: interval*increment factor
            //C0 = 0.0625 (pitch factor) / C1=0.125 / C2=0.25 / C3 = 0.5 / C4 =1
            return (rootPitch * Mathf.Pow(_pitchInterval, note)) / 10.0f;
        }

        /// <summary>
        /// Set a range of values in scale. If input array is larger or smaller than scale array, scale will be filled as fitting.
        /// </summary>
        /// <param name="pValues"></param>
        public void SetValues(bool[] pValues)
        {
            if (pValues.Length < scaleActiveNotes.Length || pValues.Length > scaleActiveNotes.Length)
                for (int i = 0; i < scaleActiveNotes.Length; i++)
                    scaleActiveNotes[i] = pValues[i];
            else
                scaleActiveNotes = pValues;
        }
    }
}