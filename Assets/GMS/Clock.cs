namespace GMS
{
    using UnityEngine;
    using System.Collections;

    [RequireComponent(typeof(AudioSource)), RequireComponent(typeof(MusicSequencer))]
    public class Clock : MonoBehaviour
    {
        public double bpm = 140.0F;

        public bool running = true;

        double elapsedTime = .0d;

        MusicSequencer musicSequencer;

        void Start()
        {
            //Init references
            musicSequencer = GetComponent<MusicSequencer>();

            //Init variables
            bpm = musicSequencer.bpm;

            //Run methods
            StartCoroutine(ClockTicker());
        }


        private void Update()
        {
            /*
            if (!running)
                return;

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 60.0d / bpm)
            {
                if (musicSequencer)
                    musicSequencer.Tick();
                elapsedTime = .0d;
            }
            */
        }

        IEnumerator ClockTicker()
        {
            while (running)
            {
                musicSequencer.Tick();
                yield return new WaitForSeconds((float)(60.0d / musicSequencer.bpm));
            }
        }

        /*
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
                //if (musicSequencer)
                //musicSequencer.Tick();
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
        }
    }
    */
    }
}