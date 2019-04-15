using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    public class AudioSourceManager : MonoBehaviour
    {
        public int totalVoices = 256;    //Total voices and respectively AudioSources available for playing sounds
        public int safeBuffer = 16;    //Number of voices reserved for fading out oldest voices once current voices reach totalVoices-safeBuffer

        // Start is called before the first frame update
        void Start()
        {
            gameObject.AddComponent<AudioSource>();
        }

        // Update is called once per frame
        public void ScheduleAudioSource()
        {
            
        }
    }
}