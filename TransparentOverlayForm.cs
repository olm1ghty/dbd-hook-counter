using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DBDtimer;
using Emgu.CV.CvEnum;
using Svg;
using Svg.Transforms;
using Timer = System.Windows.Forms.Timer;

public class TransparentOverlayForm : Form
{
    public HookManager hookManager;
    public TimerManager timerManager;

    public Survivor[] survivors = new Survivor[4];
    
    public int hookStageCounterStartX = 295;
    public int hookStageCounterStartY = 640;
    public int hookStageCounterOffset = 120;

    SvgDocument hookCounterSVG = SvgDocument.Open(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\both hooks.svg");
    Graphics graphics;
    Bitmap bmp;

    public TransparentOverlayForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        //ShowInTaskbar = false;
        TopMost = true;
        Rectangle screen = Screen.PrimaryScreen.Bounds;
        this.Bounds = screen;        // sets Location, Width, Height in one line

        StartPosition = FormStartPosition.CenterScreen;

        // Use WS_EX_LAYERED to enable per-pixel alpha
        int initialStyle = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
        NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, initialStyle | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TRANSPARENT);

        hookManager = new(this);
        timerManager = new(this);

        survivors = new Survivor[]
        {
            new Survivor(0, this),
            new Survivor(1, this),
            new Survivor(2, this),
            new Survivor(3, this)
        };

        bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
        graphics = Graphics.FromImage(bmp);
        graphics.Clear(Color.Transparent);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        hookCounterSVG.Width = new SvgUnit(SvgUnitType.Pixel, 40);
        hookCounterSVG.Height = new SvgUnit(SvgUnitType.Pixel, 40);
        hookCounterSVG.FillOpacity = 0.5f;
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

    public void DrawOverlay()
    {
        // wipe everything that was drawn last time ⟵  IMPORTANT
        graphics.Clear(Color.Transparent);

        if (hookManager.UIenabled())
        {
            // --- draw hook stages ---
            for (int i = 0; i < survivors.Length; i++)
            {
                hookCounterSVG.Fill = new SvgColourServer(Color.Transparent);

                SvgElement leftHook = hookCounterSVG.GetElementById("leftHook");
                leftHook.Fill = new SvgColourServer(Color.Transparent);

                SvgElement rightHook = hookCounterSVG.GetElementById("rightHook");
                rightHook.Fill = new SvgColourServer(Color.Transparent);

                switch (survivors[i].hookStages)
                {
                    case 0:
                        leftHook.Fill = new SvgColourServer(Color.Black);
                        rightHook.Fill = new SvgColourServer(Color.Black);
                        break;

                    case 1:
                        leftHook.Fill = new SvgColourServer(Color.White);
                        rightHook.Fill = new SvgColourServer(Color.Black);
                        break;

                    case 2:
                        leftHook.Fill = new SvgColourServer(Color.White);
                        rightHook.Fill = new SvgColourServer(Color.White);
                        break;
                }

                //Debug.WriteLine($"survivors[{i}].hookStages: {survivors[i].hookStages}");

                var state = graphics.Save();
                graphics.TranslateTransform(hookStageCounterStartX, hookStageCounterStartY + (i * hookStageCounterOffset));
                hookCounterSVG.Draw(graphics);
                graphics.Restore(state);
            }

            // clean up expired timers before drawing
            timerManager.RemoveExpiredTimers();

            // draw all active timers
            foreach (var list in timerManager.timers)
            {
                foreach (var timer in list)
                {
                    string txt = timer.SecondsRemaining.ToString();
                    using (Font f = new Font("Arial", 12, FontStyle.Bold))
                    using (Brush b = new SolidBrush(Color.Red))
                    {
                        graphics.DrawString(txt, f, b, timer.Position);
                    }
                }
            }
        }

        NativeMethods.SetBitmapToForm(this, bmp);
    }
}
