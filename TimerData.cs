public class TimerData
{
    public DateTime startedAt { get; }
    public int durationSeconds { get; }
    public Color color;

    public Point Position { get; set; }

    public int SecondsRemaining =>
        Math.Max(0, durationSeconds - (int)(DateTime.UtcNow - startedAt).TotalSeconds);

    public bool IsExpired => SecondsRemaining <= 0;

    public TimerData(int duration, Point pos, Color color)
    {
        startedAt = DateTime.UtcNow;
        durationSeconds = duration;
        Position = pos;
        this.color = color;
    }
}