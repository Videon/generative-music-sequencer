using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "SQ_", menuName = "MusicGenerator/Music Sequence", order = 1)]
public class MusicSequence : ScriptableObject
{
    [SerializeField]
    public MGSound[] sounds;

    [SerializeField]
    public Note[] notes;

    bool quantize;

    // Update method to receive time and other signals from sequencer. barTime is time position of current bar.
    public void Tick(float barTime)
    {

    }

    void PlayNote(Note note)
    {

    }
}

public struct Note
{
    public enum Modes { Single, Chord };
    public enum Length { Whole, Half, Quarter, Eighth, Sixteenth };


}