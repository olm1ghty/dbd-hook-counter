using System.Diagnostics;
using System.Runtime.InteropServices;

public class PixelOverlayForm : Form
{
    private List<List<Point>> highlightPoints = new();
    private const int pixelSize = 2;

    public PixelOverlayForm()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.Magenta; // use any distinct color
        this.TransparencyKey = Color.Magenta;
        this.TopMost = true;
        this.ShowInTaskbar = false;
        this.Bounds = Screen.PrimaryScreen.Bounds;
        this.DoubleBuffered = true;

        // Make it click-through
        int initialStyle = (int)GetWindowLong(this.Handle, -20);
        SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20); // WS_EX_LAYERED | WS_EX_TRANSPARENT
    }

    public void UpdatePoints(List<Point> points)
    {
        highlightPoints.Add(points);
        this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        foreach(var list in highlightPoints)
        {
            foreach (var pt in list)
            {
                e.Graphics.FillRectangle(Brushes.LimeGreen, pt.X, pt.Y, pixelSize, pixelSize);
            }
        }
    }

    // Win32 API for click-through
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
