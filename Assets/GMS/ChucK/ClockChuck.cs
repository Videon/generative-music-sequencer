using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GMS;
using UnityEngine;

public class ClockChuck : MonoBehaviour
{
    private ChuckSubInstance _chuckSubInstance;
    //private ChuckIntSyncer _chuckIntSyncer;
    public double bpm = 120.0d;
    private int currentInt, previousInt;

    private ChuckReader _chuckReader;

    private Thread readerThread;

    [SerializeField] private MusicSequencer _musicSequencer;

    private void Awake()
    {
        _chuckSubInstance = GetComponent<ChuckSubInstance>();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        RunClock();
    }

    public void SetClock(double pBpm)
    {
        bpm = pBpm;
        _chuckSubInstance.SetFloat("bpm", bpm);
    }

    private void Update()
    {
        //currentInt = _chuckIntSyncer.GetCurrentValue();
        previousInt = currentInt;
    }

    void RunClock()
    {
        _chuckSubInstance.RunCode(@"
			global float bpm;
            global float msTime;
            global int intCallback;
            60=>bpm;
            Impulse imp => dac;

            1::minute/bpm => dur metrotime;
            <<<""metrotime = "", metrotime/1::ms>>>;

            //timeToNext is constantly updated, so we know when the next click will happen at any moment....
                    metrotime => dur timeToNext;


            while(true) {
                1::minute/bpm =>metrotime;    //Update bpm
        
                if(timeToNext <= 0::ms) { //click now!
            
                    //click
                    1 => imp.next;
                    intCallback++;
            
                    //reset metronome
                    metrotime => timeToNext;
            
                }
        
                1::ms => now;
                timeToNext - 1::ms => timeToNext;  

            }
		");
    }

    void RunReaderThread()
    {
        _chuckReader = new ChuckReader(_chuckSubInstance, _musicSequencer);

        readerThread = new Thread(_chuckReader.ReaderThread);
        readerThread.Start();
    }

    private void OnApplicationQuit()
    {
        _chuckReader.keepRunning = false;
    }
}


public class ChuckReader
{
    //private ChuckSubInstance _chuckSubInstance;
    //private Chuck.FloatCallback _chuckFloatCallback;

    private MusicSequencer _musicSequencer;

    public bool keepRunning = true;

    public ChuckReader(ChuckSubInstance pChuckSubInstance, MusicSequencer pMusicSequencer)
    {
        //_chuckSubInstance = pChuckSubInstance;
        _musicSequencer = pMusicSequencer;
        //_chuckFloatCallback = _chuckSubInstance.CreateGetFloatCallback(_musicSequencer.Tick);
    }

    public void _ReaderThread()
    {
        while (keepRunning)
        {
            //_chuckSubInstance.GetFloat("msTime", _chuckFloatCallback);
            _musicSequencer.Tick(0.001d * (_musicSequencer.bpm / 60.0d));
            Thread.Sleep(1);
        }

        Debug.Log("stopping thread");
    }

    public void ReaderThread()
    {
        //Catch and report any exceptions here, 
        //so that Unity doesn't crash!
        try
        {
            _ReaderThread();
        }
        catch (Exception e)
        {
            if (!(e is ThreadAbortException))
                Debug.LogError("Unexpected Death: " + e.ToString());
        }
    }
}