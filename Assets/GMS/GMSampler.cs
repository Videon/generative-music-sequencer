using System;
using System.Collections;
using System.Collections.Generic;
using GMS.ScriptableObjects;
using UnityEngine;

public class GMSampler : MonoBehaviour
{
    [Tooltip("Max amount of concurrently played notes.")]
    public uint polyphony;

    //TODO: Remove SerializeField attribute when done testing.
    [SerializeField] private uint _currentSource;

    public AudioClip clip;
    private AudioSource[] _audioSources;

    public double[] notes;

    public MGSound mgSound;

    private void Awake()
    {
        //Create and assign one audiosource per possible voice.
        _audioSources = new AudioSource[polyphony];
        for (int i = 0; i < _audioSources.Length; i++)
        {
            _audioSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(double time)
    {
        AudioSource currSource = _audioSources[_currentSource % polyphony];
        currSource.clip = clip;
        currSource.PlayScheduled(time);

        _currentSource++;
    }
}