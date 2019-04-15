using UnityEngine;

namespace GMS.ScriptableObjects
{
    /// <summary>
    /// Class to store the enabled notes in a set scale.
    /// </summary>
    [CreateAssetMenu(fileName = "SCL_", menuName = "MusicGenerator/Scale", order = 1)]
    public class Scale : ScriptableObject
    {
        public bool[] scaleActiveNotes;

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