using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SND_", menuName = "MusicGenerator/Sound", order = 1)]
public class MGSound : ScriptableObject
{
    [SerializeField, Tooltip("Maximum number of notes that can be played simultaneously")]
    public int polyphony;

    [SerializeField]
    AudioClip[] sounds;

    [SerializeField]
    bool multisample;

}
