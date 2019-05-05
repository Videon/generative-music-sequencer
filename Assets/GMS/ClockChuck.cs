using System;
using System.Collections;
using System.Collections.Generic;
using GMS;
using UnityEngine;

public class ClockChuck : MonoBehaviour
{
    private ChuckSubInstance _chuckSubInstance;
    private ChuckIntSyncer _chuckIntSyncer;
    public double bpm = 120.0d;
    private int currentInt, previousInt;

    [SerializeField]
    private MusicSequencer _musicSequencer;

    private void Awake()
    {
        _chuckSubInstance = GetComponent<ChuckSubInstance>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        _chuckIntSyncer = gameObject.AddComponent<ChuckIntSyncer>();
        _chuckIntSyncer.SyncInt(_chuckSubInstance, "intCallback");

        //_musicSequencer = GetComponentInParent<MusicSequencer>();

        RunClock();
    }

    public void SetClock(double bpm)
    {
        _chuckSubInstance.SetFloat("bpm", bpm);
    }

    private void Update()
    {
        SetClock(bpm);
        currentInt = _chuckIntSyncer.GetCurrentValue();
        if (currentInt != previousInt)
        {
            _musicSequencer.Tick();
            previousInt = currentInt;
        }
    }

    void RunClock()
    {
        _chuckSubInstance.RunCode(@"
			global float bpm;
            global int intCallback;
            120=>bpm;
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
}