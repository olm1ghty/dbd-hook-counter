public class TimerData
{
    public DateTime StartedAt { get; }
    public int DurationSeconds { get; }

    public Point Position { get; set; }

    public int SecondsRemaining =>
        Math.Max(0, DurationSeconds - (int)(DateTime.UtcNow - StartedAt).TotalSeconds);

    public bool IsExpired => SecondsRemaining <= 0;

    public TimerData(int duration, Point pos)
    {
        StartedAt = DateTime.UtcNow;
        DurationSeconds = duration;
        Position = pos;
    }
}