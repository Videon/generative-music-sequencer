using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckScheduler
{
    /// <summary>Plays a given sound through ChucK.</summary>
    /// <param name="chuckSubInstance">The ChuckSubInstance that is used to schedule and play the sound</param>
    /// <param name="pFilename">The filename of the clip that will be played at the scheduled time.</param>
    /// <param name="pPitch">The pitch of the sample that will be played at the scheduled time.</param>
    public void PlaySound(ChuckSubInstance chuckSubInstance, string pFilename,
        float pPitch, float pGain)
    {
        PlayScheduled(chuckSubInstance, pPitch, pFilename, pGain);
    }

    public void PlayScheduled(ChuckSubInstance chuckSubInstance, float pitch,
        string fileName, float gain)
    {
        //PARAMETERS: {0} = scheduledTime, {0} = pitch, {1} = clipFile, {2} = gain
        chuckSubInstance.RunCode(string.Format(@"
			SndBuf sndBuf => dac;

			""{1}""=>string filename;
    		me.dir() + filename+"".wav""=> sndBuf.read;
    
    		// start at the beginning of the clip
    		0 => sndBuf.pos;  		

    		// Use this as pitch. 1=100%, 2= 200%, 0.5 = 50%;
    		{0} => sndBuf.rate;
    
    		// set gain: least intense is quiet, most intense is loud; range 0.05 to 1
    		{2} => sndBuf.gain;

    		// pass time so that the file plays
			sndBuf.length() / sndBuf.rate() => now;
    
    	", pitch, fileName, gain));
    }
}