using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;

namespace GMS.Playables
{
    public class InstrumentBehaviour : PlayableBehaviour
    {
        private GMSampler _gmSampler;
        private InstrumentClip InstrumentClip { get; set; }

        private uint _lastIndex = 0;
        private List<Note> _currentNotes;

        public override void OnGraphStart(Playable playable)
        {
            _lastIndex = 0;
            base.OnGraphStart(playable);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
            _gmSampler = playerData as GMSampler;
            if (_gmSampler == null)
                return;

            //Relative time of current clip
            double relTime = playable.GetTime() / playable.GetDuration();

            //Check if time marker has passed note.
            if (_lastIndex < _gmSampler.notes.Length)
            {
                if (_gmSampler.notes[_lastIndex] <= relTime)
                {
                    _gmSampler.PlaySound(relTime);
                    _lastIndex++;
                }
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            _lastIndex = 0;
        }
    }
}