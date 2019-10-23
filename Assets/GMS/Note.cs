public class Note
{
    public enum Modes
    {
        Single,
        Chord,
        Rhythm
    };

    public enum Length
    {
        Whole,
        Half,
        Quarter,
        Eighth,
        Sixteenth
    };

    public double barPos; //Position of current note in bar. Value between 0 and 1.

    public Modes Mode;
    public Length length;
    public float pitch;

    public Note[] chordNotes;

    public Note(Modes p_mode, Length p_length, float p_pitch)
    {
        Mode = p_mode;
        //length = p_length;
        pitch = p_pitch;
    }

    public Note(Modes p_mode, float p_pitch)
    {
        Mode = p_mode;
        pitch = p_pitch;
    }

    public Note(double pPos)
    {
        barPos = pPos;
    }
}