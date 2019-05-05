using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using GMS.ScriptableObjects;

namespace GMS
{
    [System.Serializable]
    public class MusicSequencer : MonoBehaviour
    {
        private ClockChuck _clockChuck;
        private ChuckSubInstance[] _chuckSubInstances;

        /// <summary>
        /// The amount of time note scheduling is delayed by to give the system time to load/schedule sounds
        /// </summary>
        ///<summary>Tempo of music in BPM (beats per minute)</summary>
        public double bpm = 120;

        ///<summary>The number of all steps in a bar</summary>
        public int barSteps = 16;

        ///<summary>Currently active bar</summary>
        int _currentBar = 0;

        public int currentStep = 0;

        [SerializeField] private SequenceData[] musicSequences;
        [SerializeField] private Scale[] scales;

        [SerializeField] private Vector2Int
            musicSequencesDimensions =
                new Vector2Int(0, 0); //Store "dimensions" of MusicSequences array to make it usable like a 2D array.

        private double dspTime, prevDspTime;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            _clockChuck = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockChuck>();
            _clockChuck.SetClock((float) bpm);

            _chuckSubInstances = GetComponentsInChildren<ChuckSubInstance>();
            /*for (int i = 0; i < GetMusicSequencesDimensions().y; i++)
            {
                GameObject go = GameObject.Instantiate(new GameObject(),transform);
                go.AddComponent<ChuckSubInstance>();
                ChuckSubInstance csi = go.GetComponent<ChuckSubInstance>();
                
                
                go.name = "Layer_" + (i + 1);

                csi.chuckMainInstance = GetComponentInChildren<ChuckMainInstance>();
                _chuckSubInstances.Add(csi);
            }*/
        }

        public void Tick()
        {
            if (currentStep < barSteps - 1)
                currentStep++;
            else
            {
                if (_currentBar < GetMusicSequencesDimensions().x - 1)
                    _currentBar++;
                else
                    _currentBar = 0;
                currentStep = 0;

                double currentDspTime = dspTime;

                //Generate and schedule next sequence at the beginning of the current bar
                for (var currentLayer = 0; currentLayer < GetMusicSequencesDimensions().y; currentLayer++)
                {
                    Note[] generatedSequence = GenerateSequence(GetMusicSequence(_currentBar, currentLayer));
                    ScheduleSequence(currentLayer, currentDspTime, generatedSequence,
                        GetMusicSequence(_currentBar, currentLayer));
                }
            }
        }

        ///<summary>Returns a new Note sequence based on the input SequenceData.</summary>
        private Note[] GenerateSequence(SequenceData pSequenceData)
        {
            var mode = pSequenceData.sequenceMode.ToString();

            switch (mode)
            {
                case "Legacy":
                    return SequenceGenerator.GenerateLegacy(pSequenceData, barSteps);
                case "Simple":
                    return SequenceGenerator.GenerateSimple(pSequenceData, scales[_currentBar], barSteps);
            }

            return null;
        }

        ///<summary>Schedule playing sounds for a given schedule.</summary>
        private void ScheduleSequence(int layer, double pDspTime, Note[] pSequenceNotes, SequenceData pSequenceData)
        {
            if (pSequenceNotes.Length > 0)
            {
                var stepLength = 60.0d / bpm;
                var barOffset = stepLength * (barSteps - 1);

                for (var i = 0; i < pSequenceNotes.Length; i++)
                {
                    if (pSequenceNotes[i] != null)
                    {
                        ChuckScheduler.ScheduleSound(_chuckSubInstances[layer], pDspTime + (stepLength * i),
                            pSequenceData.sound.fileName, pSequenceNotes[i].pitch);
                    }
                }
            }
        }

        /// <summary>Initialize sequences array and all other arrays that are dependant on the dimensions of bars(y) and layers(y).</summary>
        /// <param name="x">Number of bars</param>
        /// <param name="y">Number of layers</param>
        public void InitSequencer(int x, int y)
        {
            SequenceData[] oldSequences = musicSequences;
            Scale[] oldScales = scales;

            //Initialize sequences and scale
            musicSequences = new SequenceData[x * y];
            musicSequencesDimensions = new Vector2Int(x, y);

            scales = new Scale[x];

            //Fill new size with previous data
            for (int i = 0; i < oldSequences.Length; i++)
            {
                if (i >= musicSequences.Length)
                    break;
                musicSequences[i] =
                    oldSequences
                        [i]; //TODO Use separate method to account for 2d array representation in editor window. 
            }

            for (int i = 0; i < oldScales.Length; i++)
            {
                if (i >= scales.Length)
                    break;
                scales[i] = oldScales[i];
            }
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


        //Methods providing run-time information following

        public int GetCurrentBar()
        {
            return _currentBar;
        }
    }
}