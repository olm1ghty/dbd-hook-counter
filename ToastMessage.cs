using System.Diagnostics;

class ToastMessage
{
    public string Text { get; }
    public Point Position { get; }
    private readonly int durationMs;
    private readonly Stopwatch sw = Stopwatch.StartNew();

    public ToastMessage(string text, Point pos, int durationMs = 2000)
    {
        Text = text;
        Position = pos;
        this.durationMs = durationMs;
    }

    public bool IsAlive => sw.ElapsedMilliseconds < durationMs;

    /// <summary>0‑255 — fades out in the last 30 % of life</summary>
    public int CurrentAlpha
    {
        get
        {
            double t = sw.ElapsedMilliseconds / (double)durationMs;      // 0‑1
            if (t < 0.7) return 255;                                     // stay solid
            return (int)(255 * (1.0 - (t - 0.7) / 0.3));                 // linear fade
        }
    }
}
