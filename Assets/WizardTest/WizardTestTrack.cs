using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(WizardTestClip))]
[TrackBindingType(typeof(AudioSource))]
public class WizardTestTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<WizardTestMixerBehaviour>.Create (graph, inputCount);
    }
}
