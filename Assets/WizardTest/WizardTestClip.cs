using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class WizardTestClip : PlayableAsset, ITimelineClipAsset
{
    public WizardTestBehaviour template = new WizardTestBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<WizardTestBehaviour>.Create (graph, template);
        WizardTestBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
