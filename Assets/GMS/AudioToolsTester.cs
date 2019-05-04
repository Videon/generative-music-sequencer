using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMS;
using UnityEngine.UI;

public class AudioToolsTester : MonoBehaviour
{
    private AudioSource _audioSource;

    public AudioClip[] clips;
    public float[] pitches;

    public enum Modes
    {
        Stitch,
        Combine,
        Pitch
    }

    public Modes modes;

    private AudioClip result;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (modes == Modes.Stitch)
            result = Audiotools.Stitch(clips, pitches);
        else if (modes == Modes.Combine)
            result = Audiotools.Combine(clips, pitches);
        else
            result = Audiotools.PitchAudio(clips[0], pitches[0]);

        _audioSource.clip = result;
        _audioSource.Play();
    }
}