using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DBDtimer;
using Svg;
using Timer = System.Windows.Forms.Timer;

public class TransparentOverlayForm : Form
{
    public HookManager hookManager;
    public TimerManager timerManager;

    public Survivor[] survivors = new Survivor[4];

    //int seconds = 60;
    //Timer timer;
    
    private int hookStageCounterStartX = 295;
    private int hookStageCounterStartY = 640;
    private int hookStageCounterOffset = 120;

    public TransparentOverlayForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        Rectangle screen = Screen.PrimaryScreen.Bounds;
        this.Bounds = screen;        // sets Location, Width, Height in one line

        StartPosition = FormStartPosition.CenterScreen;

        // Use WS_EX_LAYERED to enable per-pixel alpha
        int initialStyle = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
        NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, initialStyle | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TRANSPARENT);

        //timer = new Timer { Interval = 1000 };
        //timer.Tick += (s, e) =>
        //{
        //    seconds--;
        //    if (seconds < 0) seconds = 30;
        //    RenderToLayeredWindow();
        //};
        //timer.Start();

        hookManager = new(this);
        timerManager = new(this);

        survivors = new Survivor[]
        {
            new Survivor(0, this),
            new Survivor(1, this),
            new Survivor(2, this),
            new Survivor(3, this)
        };

        RenderToLayeredWindow();
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TRANSPARENT;
            return cp;
        }
    }

    private void RenderToLayeredWindow()
    {
        Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.Transparent);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // --- draw all SVG hook icons ---
            for (int i = 0; i < 4; i++)
            {
                var state = g.Save();
                g.TranslateTransform(hookStageCounterStartX, hookStageCounterStartY + (i * hookStageCounterOffset));
                var svg = SvgDocument.Open(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\both hooks.svg");
                svg.Width = new SvgUnit(SvgUnitType.Pixel, 40);
                svg.Height = new SvgUnit(SvgUnitType.Pixel, 40);
                svg.Draw(g);
                g.ResetTransform();  // Reset after drawing
                g.Restore(state);
            }

            // --- draw every running timer ---
            //for (int i = 0; i < 4; i++)
            //{
            //    if (timers[i].Count > 0)
            //    {
            //        string txt = timers[i][0].SecondsLeft.ToString();
            //        using (Font f = new Font("Arial", 28, FontStyle.Bold))
            //        using (Brush b = new SolidBrush(Color.Red))
            //        {
            //            g.DrawString(txt, f, b, 235, 635 + i * 120);
            //        }
            //    }
            //}
        }

        NativeMethods.SetBitmapToForm(this, bmp);
    }

    //private void RenderToLayeredWindow()
    //{
    //    Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
    //    using (Graphics g = Graphics.FromImage(bmp))
    //    {
    //        g.Clear(Color.Transparent);
    //        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
    //        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

    //        // Load SVG
    //        var svgDoc = SvgDocument.Open(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\both hooks.svg");
    //        svgDoc.Width = Width;
    //        svgDoc.Height = Height;
    //        svgDoc.Draw(g); // Render onto transparent Graphics

    //        // Draw timer
    //        using (Font font = new Font("Arial", 28, FontStyle.Bold))
    //        using (Brush brush = new SolidBrush(Color.Red))
    //        {
    //            string timeText = seconds.ToString();
    //            SizeF textSize = g.MeasureString(timeText, font);
    //            PointF textPos = new PointF((Width - textSize.Width) / 2, (Height - textSize.Height) / 2);
    //            g.DrawString(timeText, font, brush, textPos);
    //        }
    //    }

    //    NativeMethods.SetBitmapToForm(this, bmp);
    //}
}
