using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using GMS.ScriptableObjects;
using UnityEngine.Audio;

namespace GMS
{
    [RequireComponent(typeof(ParamInterfacer))]
    public class MusicSequencer : MonoBehaviour
    {
        private ClockChuck _clockChuck;

        [SerializeField] private ChuckSubInstance[] chuckSubInstances;
        [SerializeField] private ChuckScheduler[] _chuckSchedulers;

        [SerializeField] private SequencerState workingState, tempState;

        private ParamInterfacer _paramInterfacer;

        /// <summary>If true, a copy of the working state will be created and used during run-time. Changes to the copied state will be discarded.</summary>
        public bool useWorkingCopy = true;

        ///<summary>Tempo of music in BPM (beats per minute).</summary>
        [SerializeField] public double bpm = 120;

        ///<summary>The number of all steps in a bar.</summary>
        [SerializeField] public int barSteps = 16;

        ///<summary>Currently active bar.</summary>
        int _currentBar = 0;

        /// <summary>Playback position of current bar.</summary>
        private double _currentBarTime;

        /// <summary>Array of int indexes corresponding with layer count, indicating last index of note played in layer.</summary>
        private int[] _lastPlayedNoteIndexes;

        [SerializeField] private SequenceData[] musicSequences;
        [SerializeField] private Rhythm[] rhythms;
        [SerializeField] private Scale[] scales;

        ///<summary>Store "dimensions" of MusicSequences array to make it usable like a 2D array.</summary>
        [SerializeField] private Vector2Int musicSequencesDimensions = new Vector2Int(0, 0);


        //---Following are the additions that are part of the sequencer overhaul

        /// <summary>Array of generated note arrays based on the SequenceData in the respective position.</summary>
        private Note[,][] barNotes;

        #region Runtime Initialization and Exit handling

        private void Awake()
        {
            _clockChuck = GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockChuck>();
            _clockChuck.SetClock(bpm);

            chuckSubInstances = GetComponentsInChildren<ChuckSubInstance>();
            _chuckSchedulers = new ChuckScheduler[chuckSubInstances.Length];

            _paramInterfacer = GetComponent<ParamInterfacer>();

            _lastPlayedNoteIndexes = new int[musicSequencesDimensions.y];
            _currentBarTime = barSteps + 2;

            barNotes = new Note[musicSequencesDimensions.x, musicSequencesDimensions.y][];
        }

        private void Start()
        {
            //Make sure that a working copy is always used in game builds.
#if !UNITY_EDITOR
            useWorkingCopy = true;
#endif

            //Set a temporary state to avoid overwriting the working state on runtime.
            if (useWorkingCopy)
            {
                CopyStateValues(tempState, workingState);
                workingState = tempState;
            }

            LoadStateValues();

            AssignMixerChannels();

            InitSchedulers();

            //Set to last step in last bar to have music playback start immediately from first bar
            _currentBar = GetMusicSequencesDimensions().x - 1;
        }

        void InitSchedulers()
        {
            for (int i = 0; i < _chuckSchedulers.Length; i++)
            {
                _chuckSchedulers[i] = new ChuckScheduler();
            }
        }

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            SaveStateValues();
        }
#endif

        #endregion

        #region Sequencer Persistence (state handling)

        /// <summary> Sets targetState variables to originState variables. </summary>
        /// <param name="pTargetState">The state to be modified.</param>
        /// <param name="pOriginState">The state that provides data for copying.</param>
        private void CopyStateValues(SequencerState pTargetState, SequencerState pOriginState)
        {
            pTargetState.bpm = pOriginState.bpm;
            pTargetState.barSteps = pOriginState.barSteps;
            pTargetState.musicSequences = pOriginState.musicSequences;
            pTargetState.rhythms = pOriginState.rhythms;
            pTargetState.scales = pOriginState.scales;
        }

        /// <summary> Updates the working state with current sequencer values. </summary>
        public void SaveStateValues()
        {
            workingState.bpm = bpm;
            workingState.barSteps = barSteps;

            workingState.musicSequences = musicSequences;
            workingState.rhythms = rhythms;
            workingState.scales = scales;

#if UNITY_EDITOR
            EditorUtility.SetDirty(workingState);
#endif
        }

        /// <summary> Sets the values in the sequencer to the working state values. </summary>
        public void LoadStateValues()
        {
            bpm = workingState.bpm;
            barSteps = workingState.barSteps;
            musicSequences = workingState.musicSequences;
            rhythms = workingState.rhythms;
            scales = workingState.scales;
        }

        #endregion


        //Parameter handling

        #region Parameter Handling

        /// <summary> Update all parameters for current bar </summary>
        void UpdateParameters()
        {
            for (int i = 0; i < musicSequencesDimensions.y; i++)
            {
                if (GetMusicSequence(_currentBar, i) != null)
                {
                    var seqData = GetMusicSequence(_currentBar, i);
                    for (int k = 0; k < seqData.inputs.Length; k++)
                        seqData.inputs[k].paramVal = _paramInterfacer.GetParamValue(seqData.inputs[k].paramName);
                }
            }
        }

        #endregion


        //Setting up the sequencer and routing the layers

        #region Sequencer Initialization

        /// <summary>Initialize sequences array and all other arrays that are dependant on the dimensions of bars(y) and layers(y).</summary>
        /// <param name="x">Number of bars</param>
        /// <param name="y">Number of layers</param>
        public void InitSequencer(int x, int y)
        {
            //Initialize temp object
            if (tempState == null)
            {
                tempState = ScriptableObject.CreateInstance<SequencerState>();
            }

            SequenceData[] oldSequences = musicSequences;
            Rhythm[] oldRhythms = rhythms;
            Scale[] oldScales = scales;

            //Initialize sequences and scale
            musicSequences = new SequenceData[x * y];
            musicSequencesDimensions = new Vector2Int(x, y);

            rhythms = new Rhythm[x];
            scales = new Scale[x];

            //Fill new size with previous data
            for (int i = 0; i < oldSequences.Length; i++)
            {
                if (i >= musicSequences.Length)
                    break;

                //TODO Use separate method to account for 2d array representation in editor window. 
                musicSequences[i] = oldSequences[i];
            }

            for (int i = 0; i < oldScales.Length; i++)
            {
                if (i >= scales.Length)
                    break;
                scales[i] = oldScales[i];
            }


            //Destroy previous SubInstances first if existing
            if (chuckSubInstances != null || chuckSubInstances.Length > 0)
            {
                for (int i = chuckSubInstances.Length - 1; i >= 0; i--)
                    if (chuckSubInstances[i] != null)
                        DestroyImmediate(chuckSubInstances[i].gameObject);
            }

            //Instantiate SubInstances of ChucK per layer and assign respective audio mixer tracks.
            chuckSubInstances = new ChuckSubInstance[GetMusicSequencesDimensions().y];

            for (int i = 0; i < chuckSubInstances.Length; i++)
            {
                GameObject go = new GameObject();
                go.transform.parent = transform;
                go.AddComponent<ChuckSubInstance>();
                ChuckSubInstance csi = go.GetComponent<ChuckSubInstance>();


                go.name = "Layer_" + (i + 1);

                csi.chuckMainInstance = GetComponentInChildren<ChuckMainInstance>();
                chuckSubInstances[i] = csi;
            }

            AssignMixerChannels();
        }

        void AssignMixerChannels()
        {
            for (int i = 0; i < chuckSubInstances.Length; i++)
            {
                chuckSubInstances[i].gameObject.GetComponent<AudioSource>().outputAudioMixerGroup =
                    Chuck.FindAudioMixerGroup("Layer_" + (i + 1));
            }
        }

        #endregion


        //Methods to access (read/write) information relevant for sequencing

        #region Setter and getter methods for sequencing information

        public SequencerState GetWorkingState()
        {
            return workingState;
        }

        public void SetWorkingState(SequencerState pSequencerState)
        {
            workingState = pSequencerState;
        }

        /// <summary> Returns the rhythm at given index. </summary>
        public Rhythm GetRhythm(int x)
        {
            return rhythms[x];
        }

        /// <summary> Sets the rhythm at given index. </summary>
        public void SetRhythm(int x, Rhythm pRhythm)
        {
            rhythms[x] = pRhythm;
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

        ///<summary> Return the size of the 1D array as a 2D representation. </summary>
        public Vector2Int GetMusicSequencesDimensions()
        {
            return musicSequencesDimensions;
        }

        ///<summary>Returns index of current bar.</summary>
        public int GetCurrentBar()
        {
            return _currentBar;
        }

        ///<summary>Converts and returns the current bar time for display as a float value.</summary>
        public float GetCurrentBarTimeFloat()
        {
            return (float) _currentBarTime;
        }

        #endregion


        //Methods that are run during game mode, i.e. Rhythm and Sequence generation

        #region Run-time methods

        public void Tick(double pElapsedTime)
        {
            lock (this)    //TODO: Check if locking is used correctly here and preventing crashes as intended.
            {
                _clockChuck.SetClock(bpm);
                _currentBarTime += pElapsedTime * (bpm / 60.0d);

                if (_currentBarTime >= barSteps)
                {
                    PlayNotesRemaining(_currentBar);

                    if (_currentBar < GetMusicSequencesDimensions().x - 1)
                        _currentBar++;
                    else
                        _currentBar = 0;
                    _currentBarTime = 0.0d;

                    int layerCount = GetMusicSequencesDimensions().y;

                    UpdateParameters(); //Update parameters before generating sequence

                    //Generate and schedule next sequence at the beginning of the current bar
                    for (var currentLayer = 0; currentLayer < layerCount; currentLayer++)
                    {
                        barNotes[_currentBar, currentLayer] =
                            GenerateRhythm(GetMusicSequence(_currentBar, currentLayer));

                        if (barNotes[_currentBar, currentLayer] != null &&
                            barNotes[_currentBar, currentLayer].Length > 0)
                            barNotes[_currentBar, currentLayer] = GenerateSequence(barNotes[_currentBar, currentLayer],
                                GetMusicSequence(_currentBar, currentLayer));
                    }
                }

                PlayNotes(_currentBarTime, _currentBar);
            }
        }

        private Note[] GenerateRhythm(SequenceData pSequenceData)
        {
            if (pSequenceData != null)
            {
                Rhythm rhythm = null;
                if (pSequenceData.localRhythm != null)
                    rhythm = pSequenceData.localRhythm;
                else if (rhythms[_currentBar] != null)
                {
                    rhythm = rhythms[_currentBar];
                }

                if (rhythm != null)
                {
                    Note[] rhythmOut = RhythmGenerator.GenerateRhythm(barSteps, rhythm);
                    return rhythmOut;
                }
            }

            return null;
        }

        ///<summary>Returns a new Note sequence based on the input SequenceData.</summary>
        private Note[] GenerateSequence(Note[] pGeneratedRhythm, SequenceData pSequenceData)
        {
            Scale scale = pSequenceData.localScale ? pSequenceData.localScale : scales[_currentBar];

            Note[] noteOut = SequenceGenerator.GenerateSequence(barSteps, pGeneratedRhythm, scale, pSequenceData);
            return noteOut;
        }

        private void PlayNotes(double pCurrentBarTime, int pBar)
        {
            
            for (int layer = 0; layer < musicSequencesDimensions.y; layer++)
            {
                for (;
                    _lastPlayedNoteIndexes[layer] < barNotes[pBar, layer].Length;
                    _lastPlayedNoteIndexes[layer]++)
                {
                    if (barNotes[pBar, layer][_lastPlayedNoteIndexes[layer]].barPos <= _currentBarTime)
                    {
                        SequenceData currentSequence = GetMusicSequence(pBar, layer);
                        _chuckSchedulers[layer].PlaySound(chuckSubInstances[layer],
                            currentSequence.sound.fileName, barNotes[pBar, layer][_lastPlayedNoteIndexes[layer]].pitch,
                            currentSequence.sound.gain);
                        _lastPlayedNoteIndexes[layer] += 1;
                        Debug.Log(pCurrentBarTime);
                    }
                    else return;
                }
            }
        }

        /// <summary>Plays all remaining notes in the bar and resets indexes.</summary>
        private void PlayNotesRemaining(int pBar)
        {
            for (int layer = 0; layer < musicSequencesDimensions.y; layer++)
            {
                if (barNotes[pBar, layer] != null)
                {
                    for (;
                        _lastPlayedNoteIndexes[layer] < barNotes[pBar, layer].Length;
                        _lastPlayedNoteIndexes[layer]++)
                    {
                        SequenceData currentSequence = GetMusicSequence(pBar, layer);
                        _chuckSchedulers[layer].PlaySound(chuckSubInstances[layer],
                            currentSequence.sound.fileName, barNotes[pBar, layer][_lastPlayedNoteIndexes[layer]].pitch,
                            currentSequence.sound.gain);
                    }
                }

                _lastPlayedNoteIndexes[layer] = 0;
            }
        }

        #endregion
    }
}