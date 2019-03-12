using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class MusicSequencer : MonoBehaviour
{
    [SerializeField, Tooltip("Tempo of music in BPM (beats per minute)")]
    public int tempo = 100;

    private static AudioSource[] audioSources;

    [SerializeField]
    private MusicSequence[] musicSequences;
    [SerializeField]
    private Vector2Int musicSequencesDimensions = new Vector2Int(0, 0);    //Store "dimensions" of MusicSequences array to make it usable like a 2D array.


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSources = new AudioSource[DetermineAudioSources()];
        AddAudioSources();
    }

    //Determine number of audio sources needed. Current calculation is based on number of different sounds per sequence.
    int DetermineAudioSources()
    {
        int totalSoundClipsCount = 0;
        for (int x = 0; x < musicSequences.Length; x++)
        {
            if (musicSequences[x] != null)
            {
                for (int i = 0; i < musicSequences[x].sounds.Length; i++)
                    totalSoundClipsCount += musicSequences[x].sounds[i].polyphony;
            }
        }
        return totalSoundClipsCount;
    }

    void AddAudioSources()
    {
        int audioSourceIndex = 0;

        for (int x = 0; x < musicSequences.Length; x++)
        {
            if (musicSequences[x] != null)
            {
                audioSources[audioSourceIndex] = gameObject.AddComponent<AudioSource>();
                audioSourceIndex++;
            }
        }
    }

    public void InitSequences(int x, int y)
    {
        musicSequences = new MusicSequence[x * y];
        musicSequencesDimensions = new Vector2Int(x, y);
    }

    public MusicSequence GetMusicSequence(int x, int y)
    {
        return musicSequences[y * musicSequencesDimensions.x + x];
    }

    public void SetMusicSequence(int x, int y, MusicSequence musicSequence)
    {
        musicSequences[y * musicSequencesDimensions.x + x] = musicSequence;
    }

    //Return the size of the 1D array as a 2D representation
    public Vector2Int GetMusicSequencesDimensions()
    {
        return musicSequencesDimensions;
    }

    void PlaySequence()
    {

    }
}
