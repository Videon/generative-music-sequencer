using System;
using GMS.Playables;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class InstrumentClip : PlayableAsset, ITimelineClipAsset
{
    private InstrumentBehaviour template = new InstrumentBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<InstrumentBehaviour>.Create(graph, template);
    }
}