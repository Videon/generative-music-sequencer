using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    public class AudioSourceManager : MonoBehaviour
    {
        //Total voices and respectively AudioSources available for playing sounds
        public int totalVoices = 64;

        //Number of voices reserved for fading out oldest voices once current voices reach totalVoices-safeBuffer
        public int safeBuffer = 6;

        private List<AudioSource> _audioSources;

        // Start is called before the first frame update
        void Start()
        {
            _audioSources = new List<AudioSource>();
            for (int i = 0; i < totalVoices; i++)
            {
                AudioSource temp = gameObject.AddComponent<AudioSource>();
                temp.hideFlags = HideFlags.HideInInspector;

                _audioSources.Add(temp);
                _audioSources[i].playOnAwake = false;
            }
        }

        /// <summary> Assign, move to back, configure  and schedule AudioSource to be used for sound playback.</summary>
        /// <param name="pScheduledTime">The absolute time when the sound is scheduled to be played.</param>
        /// <param name="pClip">The sample clip that will be played at the scheduled time.</param>
        /// <param name="pPitch">The pitch of the sample that will be played at the scheduled time.</param>
        public void ScheduleAudioSource(double pScheduledTime, AudioClip pClip, float pPitch)
        {
            AudioSource audioSource = _audioSources[0];
            _audioSources.RemoveAt(0);
            _audioSources.Add(audioSource);

            audioSource.PlayScheduled(pScheduledTime);
            audioSource.clip = pClip;
            audioSource.pitch = pPitch;
        }
    }
}