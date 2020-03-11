using UnityEngine;
using UnityEngine.Timeline;

namespace GMS.Playables
{
    [TrackColor(1, 1, 1)]
    [TrackBindingType(typeof(GMSampler))]
    [TrackClipType(typeof(InstrumentClip))]
    public class InstrumentTrack : TrackAsset
    {
    }
}