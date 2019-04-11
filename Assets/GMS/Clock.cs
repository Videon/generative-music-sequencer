namespace GMS
{
    using UnityEngine;
    using System.Collections;
    // The code example shows how to implement a metronome that procedurally generates the click sounds via the OnAudioFilterRead callback.
    // While the game is paused or the suspended, this time will not be updated and sounds playing will be paused. Therefore developers of music scheduling routines do not have to do any rescheduling after the app is unpaused

    [RequireComponent(typeof(AudioSource)), RequireComponent(typeof(MusicSequencer))]
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
        public bool running = true;

        double elapsedTime = .0d;

        MusicSequencer musicSequencer;

        void Start()
        {
            //Init variables
            bpm = musicSequencer.bpm;

            //Init references
            musicSequencer = GetComponent<MusicSequencer>();

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