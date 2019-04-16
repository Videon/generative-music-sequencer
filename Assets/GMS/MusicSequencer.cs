using UnityEngine;
using UnityEditor;
using System.IO;
using GMS.ScriptableObjects;

namespace GMS
{
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
        [SerializeField] private Scale[] scales;

        [SerializeField] private Vector2Int
            musicSequencesDimensions =
                new Vector2Int(0, 0); //Store "dimensions" of MusicSequences array to make it usable like a 2D array.

        private double dspTime, prevDspTime;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            for (int i = 0; i < 256; i++)
                gameObject.AddComponent<AudioSource>();
        }

        // Start is called before the first frame update
        void Start()
        {
            _audioSources = GetComponents<AudioSource>();
        }

        public void SetDspTime(double pDspTime)
        {
            dspTime = pDspTime;
        }

        void Update()
        {
            if (dspTime != prevDspTime)
            {
                prevDspTime = dspTime;
                Tick();
            }
        }

        public void Tick()
        {
            print("BAR> " + (_currentBar + 1));
            print("STEP> " + (currentStep + 1));

            if (currentStep < barSteps - 1)
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

        ///<summary>Returns a new Note sequence based on the input SequenceData.</summary>
        private Note[] GenerateSequence(SequenceData pSequenceData)
        {
            var mode = pSequenceData.sequenceMode.ToString();
            ;
            switch (mode)
            {
                case "Legacy":
                    return SequenceGenerator.GenerateLegacy(pSequenceData, barSteps);
                case "Simple":
                    return SequenceGenerator.GenerateSimple(pSequenceData, scales[_currentBar], barSteps);
            }

            return null;
        }

        ///<summary>Schedule playing sounds for a given schedule</summary>
        private void ScheduleSequence(Note[] pSequenceNotes, SequenceData pSequenceData)
        {
            var stepLength = 60.0d / bpm;
            var barOffset = stepLength * (barSteps - 1);
            var currentDspTime = dspTime;

            for (var i = 0; i < pSequenceNotes.Length; i++)
            {
                if (pSequenceNotes[i] != null)
                {
                    _audioSources[i].PlayScheduled((currentDspTime) + (stepLength * i));
                    _audioSources[i].clip = pSequenceData.sound.sounds[0];
                    _audioSources[i].pitch = pSequenceNotes[i].pitch;
                    print("scheduling " + i);
                }
            }
        }

        /// <summary> Initialize sequences array and all other arrays that are dependant on the dimensions of bars(y) and layers(y)</summary>\
        public void InitSequencer(int x, int y)
        {
            musicSequences = new SequenceData[x * y];
            musicSequencesDimensions = new Vector2Int(x, y);

            scales = new Scale[x];
        }

        /// <summary> Returns the music sequence at the given x y position in the visible grid. Converts input x y into a 1d coordinate for lookup in original array. </summary>
        public SequenceData GetMusicSequence(int x, int y)
        {
            return musicSequences[y * musicSequencesDimensions.x + x];
        }

        /// <summary> Sets the music sequence at given index. </summary>
        public void SetMusicSequence(int x, int y, SequenceData pMusicSequence)
        {
            musicSequences[y * musicSequencesDimensions.x + x] = pMusicSequence;
        }

        /// <summary> Returns the scale object at given index. </summary>
        public Scale GetScale(int x)
        {
            return scales[x];
        }

        /// <summary> Sets the scale at given index. </summary>
        public void SetScale(int x, Scale pScale)
        {
            scales[x] = pScale;
        }

        ///<summary> Return the size of the 1D array as a 2D representation. </summary>
        public Vector2Int GetMusicSequencesDimensions()
        {
            return musicSequencesDimensions;
        }
    }
}