using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChuckScheduler
{
    /// <summary> Assign, move to back, configure  and schedule AudioSource to be used for sound playback.</summary>
    /// <param name="chuckSubInstance">The ChuckSubInstance that is used to schedule and play the sound</param>
    /// <param name="pScheduledTime">The absolute time when the sound is scheduled to be played.</param>
    /// <param name="pClip">The sample clip that will be played at the scheduled time.</param>
    /// <param name="pPitch">The pitch of the sample that will be played at the scheduled time.</param>
    public static void ScheduleSound(ChuckSubInstance chuckSubInstance, double pScheduledTime, AudioClip pClip,
        float pPitch)
    {
        PlayScheduled(chuckSubInstance, pScheduledTime, pPitch, GetFileName(pClip));
    }

    static string GetFileName(AudioClip pClip)
    {
        return pClip.name + ".wav";
    }

    public static void PlayScheduled(ChuckSubInstance chuckSubInstance, double scheduledTime, float pitch,
        string fileName)
    {
        //PARAMETERS: {0} = scheduledTime, {1} = pitch, {2} = clipFile
        chuckSubInstance.RunCode(string.Format(@"
    		now +{0}::second =>time later;
			SndBuf sndBuf => dac;

			//Wait a given amount of time
			while(now<later){{
				10::ms=>now;
			}}


    		me.dir() + {2} => sndBuf.read;
    
    		// start at the beginning of the clip
    		0 => sndBuf.pos;  		

    		// Use this as pitch. 1=100%, 2= 200%, 0.5 = 50%;
    		{1} => sndBuf.rate;
    
    		// set gain: least intense is quiet, most intense is loud; range 0.05 to 1
    		0.05 + 0.95 => sndBuf.gain;
    
			

    		// pass time so that the file plays
			sndBuf.length() / sndBuf.rate() => now;
    
    	", scheduledTime, pitch, fileName));
    }
}