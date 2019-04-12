namespace GMS
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;

    [System.Serializable]
    public class MusicSequencer : MonoBehaviour
    {
        ///<summary>Tempo of music in BPM (beats per minute)</summary>
        public double bpm = 60;

        ///<summary>The number of all steps in a bar</summary>
        public int barSteps = 16;

        ///<summary>Currently active bar</summary>
        int _currentBar = 0;

        public int currentStep = 0;

        private static AudioSource[] _audioSources;
        AudioSource _audioSource;

        [SerializeField] private SequenceData[] musicSequences;

        [SerializeField] private Vector2Int
            musicSequencesDimensions =
                new Vector2Int(0, 0); //Store "dimensions" of MusicSequences array to make it usable like a 2D array.

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            _audioSources = GetComponents<AudioSource>();
        }

        public void Tick()
        {
            print("BAR> " + _currentBar);
            print("STEP> " + currentStep);

            if (currentStep < barSteps)
                currentStep++;
            else
            {
                if (_currentBar < GetMusicSequencesDimensions().x - 1)
                    _currentBar++;
                else
                    _currentBar = 0;
                currentStep = 0;

                //Generate and schedule next sequence at the beginning of the current bar
                for (var currentLayer = 0; currentLayer < GetMusicSequencesDimensions().y; currentLayer++)
                {
                    ScheduleSequence(GenerateSequence(GetMusicSequence(_currentBar, currentLayer)),
                        GetMusicSequence(_currentBar, currentLayer));
                }
            }
        }

        private Note[] GenerateSequence(SequenceData pSequenceData)
        {
            var mode = pSequenceData.sequenceMode.ToString();
            ;
            switch (mode)
            {
                case "Legacy":
                    return SequenceGenerator.GenerateLegacy(pSequenceData, barSteps);
            }

            return null;
        }

        //Schedule playing sounds for a given schedule
        private void ScheduleSequence(Note[] pSequenceNotes, SequenceData pSequenceData)
        {
            var stepLength = 60.0d / bpm;
            var barOffset = stepLength * barSteps;
            var dspTime = AudioSettings.dspTime;

            for (var i = 0; i < pSequenceNotes.Length; i++)
            {
                if (pSequenceNotes[i] != null)
                {
                    _audioSources[i].PlayScheduled((dspTime + 0.1f) + (stepLength * i));
                    _audioSources[i].clip = pSequenceData.sound.sounds[0];
                    _audioSources[i].pitch = pSequenceNotes[i].pitch;
                }
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
    }
}