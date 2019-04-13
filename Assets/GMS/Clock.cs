using UnityEditor;

namespace GMS
{
    using UnityEngine;
    using System.Collections;

    [RequireComponent(typeof(AudioSource))]
    public class Clock : MonoBehaviour
    {
        public double bpm = 140.0F;
        public float gain = 0.5F;
        public int signatureHi = 4;
        public int signatureLo = 4;
        private double nextTick = 0.0F;
        private float amp = 0.0F;
        private float phase = 0.0F;
        private double sampleRate = 0.0F;
        private int accent;

        public bool running = false;

        MusicSequencer musicSequencer;

        void Start()
        {
            //Init references
            musicSequencer = GetComponentInParent<MusicSequencer>();

            //Init variables
            bpm = musicSequencer.bpm;

            accent = signatureHi;
            double startTick = AudioSettings.dspTime;
            sampleRate = AudioSettings.outputSampleRate;
            nextTick = startTick * sampleRate;
            running = true;
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (!running)
                return;

            double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
            double sample = AudioSettings.dspTime * sampleRate;
            int dataLen = data.Length / channels;
            int n = 0;
            while (n < dataLen)
            {
                float x = gain * amp * Mathf.Sin(phase);
                int i = 0;
                while (i < channels)
                {
                    data[n * channels + i] += x;
                    i++;
                }

                while (sample + n >= nextTick)
                {
                    nextTick += samplesPerTick;
                    amp = 1.0F;
                    if (++accent > signatureHi)
                    {
                        accent = 1;
                        amp *= 2.0F;
                    }

                    if (musicSequencer)
                        musicSequencer.SetDspTime(AudioSettings.dspTime);
                }

                phase += amp * 0.3F;
                amp *= 0.993F;
                n++;
            }
        }
    }
}