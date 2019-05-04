using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    public static class Audiotools
    {
        public static AudioClip Stitch(AudioClip[] clips, float[] pitches)
        {
            //Check if parameter array is not empty
            if (clips == null || clips.Length == 0)
                return null;

            //Calculate total length of added clips
            int length = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] == null)
                    continue;

                length += clips[i].samples * clips[i].channels;
            }

            float[] data = new float[length]; //Declare data stream with new total length
            length = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] == null)
                    continue;

                float[]
                    buffer = new float[clips[i].samples *
                                       clips[i].channels]; //Temporary buffer to be copied into new combined data stream
                clips[i].GetData(buffer, 0);
                //System.Buffer.BlockCopy(buffer, 0, data, length, buffer.Length);
                buffer.CopyTo(data, length);
                length += buffer.Length;
            }

            if (length == 0)
                return null;

            AudioClip result = AudioClip.Create("Combine", length / 2, 2, 44100, false, false);
            result.SetData(data, 0);

            return result;
        }

        public static AudioClip Combine(AudioClip[] clips, float[] pitches)
        {
            //Check if parameter array is not empty
            if (clips == null || clips.Length == 0)
                return null;


            for (int i = 0; i < clips.Length; i++)
                clips[i] = PitchAudio(clips[i], pitches[i]);
            
            //Determine longest clip and set new length accordingly
            int length = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] == null)
                    continue;
                int currentLength = clips[i].samples * clips[i].channels;
                if (currentLength > length)
                    length = currentLength;
            }

            //Declare jagged array to store sounds that are to be played simultaneously
            float[][] inputData = new float[clips.Length][];
            for (int i = 0; i < inputData.Length; i++)
            {
                inputData[i] = new float[clips[i].samples * clips[i].channels];
                clips[i].GetData(inputData[i], 0);
            }

            float[] outputData = new float[length]; //Declare data stream with new total length
            for (int i = 0; i < clips.Length; i++)
            {
                for (int k = 0; k < clips[i].samples * clips[i].channels; k++)
                    outputData[k] += inputData[i][k];
            }

            if (length == 0)
                return null;

            AudioClip result = AudioClip.Create("Combine", length / 2, 2, 44100, false, false);
            result.SetData(outputData, 0);

            return result;
        }

        public static AudioClip PitchAudio(AudioClip clip, float pitch)
        {
            if (clip == null)
                return null;

            float[] inputData = new float[clip.samples * clip.channels];
            clip.GetData(inputData, 0);

            //Calculate target length of pitched result
            int length = 0;
            length = (int) (inputData.Length / pitch);

            float[] outputData = new float[length]; //Declare data stream with new total length
            for (int i = 0; i < inputData.Length; i += 2)
            {
                outputData[(int) (i / pitch)] = inputData[i];
            }

            for (int i = 1; i < inputData.Length; i += 2)
            {
                outputData[(int) (i / pitch)] = inputData[i];
            }

            AudioClip result = AudioClip.Create("Pitch", length / 2, 2, 44100, false, false);
            result.SetData(outputData, 0);

            return result;
        }
    }
}