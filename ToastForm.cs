using System.Diagnostics;

public sealed class ToastForm : Form
{
    private readonly System.Windows.Forms.Timer fadeTimer = new() { Interval = 16 };
    private readonly int lifeMs;
    private readonly Stopwatch sw = Stopwatch.StartNew();

    public ToastForm(string text, Point screenPos, int durationMs = 2000)
    {
        lifeMs = durationMs;

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        DoubleBuffered = true;
        BackColor = Color.Black;
        Opacity = 0;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        var label = new Label
        {
            Text = text,
            AutoSize = true,
            ForeColor = Color.Yellow,
            BackColor = Color.Transparent,
            Font = new Font("Arial", 16, FontStyle.Bold),
            Padding = new Padding(10),
        };
        Controls.Add(label);

        Location = screenPos;

        // WS_EX_NOACTIVATE & layered transparency
        const int WS_EX_NOACTIVATE = 0x08000000;
        int exStyle = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
        NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE,
                                    exStyle | WS_EX_NOACTIVATE | NativeMethods.WS_EX_LAYERED);

        fadeTimer.Tick += (_, __) => Animate();
        fadeTimer.Start();
    }

    private void Animate()
    {
        double t = sw.ElapsedMilliseconds / (double)lifeMs;

        if (t <= 0.15)                    // fade‑in 15 %
            Opacity = t / 0.15;
        else if (t >= 0.8)                // fade‑out last 20 %
            Opacity = Math.Max(0, 1 - (t - 0.8) / 0.2);
        else
            Opacity = 1;

        if (t >= 1)
        {
            fadeTimer.Stop();
            Close();
        }
    }

    protected override bool ShowWithoutActivation => true;
}
