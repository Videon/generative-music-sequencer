namespace GMS
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;

    [System.Serializable]
    public class MusicSequencer : MonoBehaviour
    {
        [SerializeField, Tooltip("Tempo of music in BPM (beats per minute)")]
        public double bpm = 60;

        [SerializeField, Tooltip("The number of all steps in a bar")]
        int barSteps = 16;

        int currentBar = 1;

        public int currentStep = 15;

        private static AudioSource[] audioSources;
        AudioSource audioSource;

        [SerializeField]
        private SequenceData[] musicSequences;
        [SerializeField]
        private Vector2Int musicSequencesDimensions = new Vector2Int(0, 0);    //Store "dimensions" of MusicSequences array to make it usable like a 2D array.

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            audioSources = GetComponents<AudioSource>();
        }

        public void Tick()
        {

            print("BAR> " + currentBar);
            print("STEP> " + currentStep);

            if (currentStep < barSteps)
                currentStep++;
            else
            {
                if (currentBar < GetMusicSequencesDimensions().x - 1)
                    currentBar++;
                else
                    currentBar = 0;
                currentStep = 0;

                //Generate and schedule next sequence at the beginning of the current bar
                for (int currentLayer = 0; currentLayer < GetMusicSequencesDimensions().y; currentLayer++)
                {
                    ScheduleSequence(GenerateSequence(GetMusicSequence(currentBar, currentLayer)));
                }
            }
        }

        Note[] GenerateSequence(SequenceData p_sequenceData)
        {
            string mode = p_sequenceData.sequenceMode.ToString(); ;
            switch (mode)
            {
                case "Legacy":
                    return SequenceGenerator.GenerateLegacy(p_sequenceData, barSteps);
            }
            return null;
        }

        //Schedule playing sounds for a given schedule
        void ScheduleSequence(Note[] p_sequenceNotes)
        {
            double stepLength = 60.0d / bpm;
            double barOffset = stepLength * barSteps;
            for (int i = 0; i < p_sequenceNotes.Length; i++)
            {
                if (p_sequenceNotes[i] != null)
                    audioSources[i].PlayScheduled((AudioSettings.dspTime + barOffset) + (stepLength * i));
            }
        }

        public void InitSequences(int x, int y)
        {
            musicSequences = new SequenceData[x * y];
            musicSequencesDimensions = new Vector2Int(x, y);
        }

        public SequenceData GetMusicSequence(int x, int y)
        {
            return musicSequences[y * musicSequencesDimensions.x + x];
        }

        public void InitMusicSequences(int x, int y, SequenceData musicSequence)
        {
            musicSequences[y * musicSequencesDimensions.x + x] = musicSequence;
        }

        //Return the size of the 1D array as a 2D representation
        public Vector2Int GetMusicSequencesDimensions()
        {
            return musicSequencesDimensions;
        }

        /*

        private void FixedUpdate()
        {
            if (localTick != globalTick)
            {
                localTick = globalTick;
                //"Forward signal" to play sequences
                PlayBar();
            }
        }

        //Determine number of audio sources needed. Current calculation is based on number of different sounds per sequence.
        int DetermineAudioSources()
        {
            int totalSoundClipsCount = 0;
            for (int x = 0; x < musicSequences.Length; x++)
            {
                if (musicSequences[x] != null)
                {
                    //for (int i = 0; i < musicSequences[x].sounds.Length; i++)
                    totalSoundClipsCount++;
                }
            }
            return totalSoundClipsCount;
        }

        void AddAudioSources()
        {
            int audioSourceIndex = 0;

            for (int x = 0; x < musicSequences.Length; x++)
            {
                if (musicSequences[x] != null)
                {
                    audioSources[audioSourceIndex] = gameObject.AddComponent<AudioSource>();
                    audioSourceIndex++;
                }
            }
        }

        void PlayBar()
        {
            for (int y = 0; y < musicSequencesDimensions.y; y++)
            {
                PlaySequence(currentBar, y);
            }
        }

        void PlaySequence(int x, int y)
        {
            if (GetMusicSequence(x, y) != null)
            {
                MusicSequence currentSequence = GetMusicSequence(x, y);
                PlaySound(currentSequence.ReturnNoteData(localTick), currentSequence.sound);
            }
        }

        void PlaySound(Note p_note, MGSound p_sound)
        {

            //audioSource.clip = p_sound.sounds[0];   //TODO: Add support for multisamples
            audioSource.PlayScheduled(AudioSettings.dspTime);   //TODO: Schedule all sounds in sequence
        }

        public void Tick()
        {
            if (globalTick < barSteps)
                globalTick++;
            else
            {
                if (currentBar < musicSequencesDimensions.x)
                    currentBar++;
                else
                    currentBar = 0;

                globalTick = 0;
            }

            sequencerPos++;
        }

        void GenerateSequence(MusicSequence sequence)
        {

        }
        */
    }
}