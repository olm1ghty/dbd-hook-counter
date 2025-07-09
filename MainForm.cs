using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Timers;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Reg;
using Emgu.CV.Flann;
using Svg;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace DBDtimer
{
    public partial class MainForm : Form
    {
        // Import the SetWindowPos function from the Windows API
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Constants for SetWindowPos
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_SHOWWINDOW = 0x0040;

        // Add a list to store HookStageCounter instances for each survivor
        private List<HookStageCounter> hookStageCounters = new List<HookStageCounter>();
        private List<TimerControl>[] timers = new List<TimerControl>[4];

        private int hook = 11;
        private int styptic = 5;
        private int ds = 61;

        private int timerStartXoffset = 235;
        private int timerStartYoffset = -5;
        private int timerDistanceYoffset = 10;

        private int hookStageCounterStartX = 295;
        private int hookStageCounterStartY = 640;
        private int hookStageCounterOffset = 120;

        private System.Timers.Timer screenMonitorTimer;
        private Point safetyPixel1 = new Point(170, 1180);
        private Point safetyPixel2 = new Point(235, 1284);
        private Color safetyPixel1Color = Color.White;
        private Color safetyPixel2Color = Color.Black;
        private int safetyPixel1tolerance = 150;
        private int safetyPixel2tolerance = 100;
        int whiteThreshold = 20;

        private const int checkIntervalMs = 1000;
        bool surivorHooked = false;
        private Survivor[] survivors = new Survivor[4];

        private PixelOverlayForm overlay;

        public Image<Bgr, byte> hookedTemplate;
        public Image<Bgr, byte> nextStageTemplate;

        public MainForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.AllowTransparency = true;
            this.BackColor = Color.Magenta;            // Any unique color
            this.TransparencyKey = Color.Magenta;      // Same as BackColor
            this.TopMost = true;                       // Optional overlay behavior
            this.DoubleBuffered = true;

            hookedTemplate = new Image<Bgr, byte>(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\States\Hooked.png");
            nextStageTemplate = new Image<Bgr, byte>(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\States\next_stage.png");

            //overlay = new PixelOverlayForm();
            //overlay.Show();

            survivors = new Survivor[]
            {
                new Survivor(0, this),
                new Survivor(1, this),
                new Survivor(2, this),
                new Survivor(3, this)
            };

            //LaunchDBD();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.LightGray;
            this.TransparencyKey = Color.LightGray; // Set TransparencyKey to the same color as BackColor
            this.TopMost = true;
            this.KeyPreview = true;
            this.KeyUp += MainForm_KeyUp;

            // Initialize timer lists
            for (int i = 0; i < 4; i++)
            {
                timers[i] = new List<TimerControl>();
            }

            // Create HookStageCounter instances for each survivor and add them to the form
            for (int i = 0; i < 4; i++)
            {
                HookStageCounter hookStageCounter = new HookStageCounter();
                hookStageCounter.Location = new Point(hookStageCounterStartX, hookStageCounterStartY + i * hookStageCounterOffset); // Adjust location as needed
                hookStageCounter.Size = new Size(200, 50); // Adjust size as needed
                this.Controls.Add(hookStageCounter);
                hookStageCounters.Add(hookStageCounter);
            }

            // Set up the global keyboard hook
            KeyboardHook.KeyPressed += KeyboardHook_KeyPressed;
            KeyboardHook.SetHook();

            StartHookDetection();

            // Handle the Activated event to ensure the form stays on top
            this.Activated += MainForm_Activated;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var svg = SvgDocument.Open(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\both hooks.svg");
            svg.Width = this.Width;
            svg.Height = this.Height;

            // Create a transparent bitmap
            using (var bmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent); // ensures real alpha
                svg.Draw(g); // render directly to transparent surface

                e.Graphics.CompositingMode = CompositingMode.SourceOver;
                e.Graphics.DrawImage(bmp, 0, 0);
            }
        }

        private void StartHookDetection()
        {
            screenMonitorTimer = new System.Timers.Timer(checkIntervalMs);
            screenMonitorTimer.Elapsed += ScreenMonitorTimer_Elapsed;
            screenMonitorTimer.Start();
        }

        private void ScreenMonitorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (UIenabled())
                {
                    foreach (var survivor in survivors)
                    {
                        this.Invoke(new Action(() =>
                        {
                            survivor.Update();
                        }));
                    }
                }
            }
            catch { }
        }

        public static Bitmap CaptureRegion(Rectangle region)
        {
            Bitmap bmp = new Bitmap(region.Width, region.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(region.Location, Point.Empty, region.Size);
            }
            return bmp;
        }

        private bool UIenabled()
        {
            bool enabled = false;

            Color currentColor1 = GetColorAt(safetyPixel1);
            Color currentColor2 = GetColorAt(safetyPixel2);

            int diff1 = Math.Abs(currentColor1.R - currentColor1.G);
            int diff2 = Math.Abs(currentColor1.R - currentColor1.B);

            if (diff1 <= whiteThreshold
                && diff2 <= whiteThreshold

                && currentColor1.R >= 255 - safetyPixel1tolerance
                && currentColor1.G >= 255 - safetyPixel1tolerance
                && currentColor1.B >= 255 - safetyPixel1tolerance

                && currentColor2.R <= 0 + safetyPixel2tolerance
                && currentColor2.G <= 0 + safetyPixel2tolerance
                && currentColor2.B <= 0 + safetyPixel2tolerance)
            {
                enabled = true;
            }

            //Debug.WriteLine($"uiCheck currentColor1 = {currentColor1}");
            //Debug.WriteLine($"uiCheck currentColor2 = {currentColor2}");
            //Debug.WriteLine($"uiCheck passed? = {enabled}");

            return enabled;
        }

        public static Color GetColorAt(Point location)
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(location, Point.Empty, new Size(1, 1));
                }
                return bmp.GetPixel(0, 0);
            }
        }

        public Color GetAverageColorAt(Point location, string debugPath = null)
        {
            // highlight target area for debug
            //var debugPoints = new List<Point>();
            //for (int x = 0; x < 2; x++)
            //{
            //    for (int y = 0; y < 2; y++)
            //    {
            //        debugPoints.Add(new Point(location.X + x, location.Y + y));
            //    }
            //}
            //overlay.UpdatePoints(debugPoints);

            // Define the size of the region (2x2)
            int regionWidth = 2;
            int regionHeight = 2;

            // Get full screen bounds (adjust if using multiple monitors)
            Rectangle screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            // Capture full screen
            using (Bitmap fullBmp = new Bitmap(screenBounds.Width, screenBounds.Height))
            {
                using (Graphics g = Graphics.FromImage(fullBmp))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, screenBounds.Size);
                }

                // Calculate average color from the 2x2 region
                int totalR = 0, totalG = 0, totalB = 0;
                for (int x = 0; x < regionWidth; x++)
                {
                    for (int y = 0; y < regionHeight; y++)
                    {
                        int px = location.X + x;
                        int py = location.Y + y;

                        // Skip if out of bounds
                        if (px < 0 || px >= fullBmp.Width || py < 0 || py >= fullBmp.Height)
                            continue;

                        Color pixel = fullBmp.GetPixel(px, py);
                        totalR += pixel.R;
                        totalG += pixel.G;
                        totalB += pixel.B;

                        // Draw a green pixel to indicate it's sampled
                        fullBmp.SetPixel(px, py, Color.Lime);
                    }
                }

                // Save debug image with green overlay
                if (debugPath != null)
                {
                    fullBmp.Save(debugPath);
                }

                // Compute average
                int count = regionWidth * regionHeight;
                return Color.FromArgb(totalR / count, totalG / count, totalB / count);
            }
        }

        private Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            return bitmap;
        }


        public bool MatchTemplate(Image<Bgr, byte> template, Rectangle region, double threshold = 0.9)
        {
            Bitmap screen = CaptureScreen();

            Image<Bgr, byte> image = screen.ToImage<Bgr, byte>();
            var roi = image.GetSubRect(region);
            using (var result = roi.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                result.MinMax(out _, out double[] maxVals, out _, out Point[] maxLocs);
                return maxVals[0] >= threshold;
            }
        }


        private void MainForm_Activated(object sender, EventArgs e)
        {
            // Bring the form to the top of other windows
            SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
        }

        private void KeyboardHook_KeyPressed(object sender, KeyEventArgs e)
        {
            // Handle the key press event
            MainForm_KeyUp(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                // Your existing disposal code...

                // Unhook the keyboard hook
                KeyboardHook.Unhook();
            }
            base.Dispose(disposing);
        }

        private Process dbdProcess;

        private void LaunchDBD()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\user\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Steam\Dead by Daylight.url";
            startInfo.UseShellExecute = true;

            try
            {
                dbdProcess = Process.Start(startInfo);

                // Wait a few seconds to allow process to appear in Task Manager
                Task.Run(() =>
                {
                    Thread.Sleep(3000);
                    MonitorDBD();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not launch DBD: " + ex.Message);
            }
        }

        private void MonitorDBD()
        {
            while (true)
            {
                // Look for DBD by process name
                var dbdProcesses = Process.GetProcessesByName("DeadByDaylight");
                if (dbdProcesses.Length == 0)
                {
                    // DBD has exited — close this app
                    Invoke(new Action(() =>
                    {
                        this.Close();
                    }));
                    break;
                }

                Thread.Sleep(3000); // Check every 3 seconds
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            // Bring the form to the top of other windows
            SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        TriggerTimer(0);
                        break;

                    case Keys.D2:
                        TriggerTimer(1);
                        break;

                    case Keys.D3:
                        TriggerTimer(2);
                        break;

                    case Keys.D4:
                        TriggerTimer(3);
                        break;

                    case Keys.D5:
                        ClearAllTimers();
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.X:
                        AddTimer(styptic);
                        break;
                    case Keys.D1:
                        AddHookStage(0);
                        break;
                    case Keys.D2:
                        AddHookStage(1);
                        break;
                    case Keys.D3:
                        AddHookStage(2);
                        break;
                    case Keys.D4:
                        AddHookStage(3);
                        break;
                    case Keys.D5:
                        ResetHookStages();
                        break;
                }
            }
        }

        public void TriggerTimer(int index)
        {
            if (TimerExists(index))
            {
                RemoveTimer(index);
            }
            else
            {
                AddTimer(hook, index);
                AddTimer(ds, index);
            }
        }

        public void UnhookSurvivor(int index)
        {
            survivors[index].SwitchState(survivors[index].stateUnhooked);
        }

        private void ResetHookStages()
        {
            foreach (HookStageCounter hookStageCounter in hookStageCounters)
            {
                hookStageCounter.HookStages = 0;
            }
        }

        public void AddHookStage(int survivorIndex)
        {
            HookStageCounter hookStageCounter = hookStageCounters[survivorIndex];

            if (hookStageCounter.HookStages < 2)
            {
                hookStageCounter.HookStages++;
            }
            else
            {
                hookStageCounter.HookStages = 0;
            }

            if (TimerExists(survivorIndex))
            {
                RemoveTimer(survivorIndex);
            }
        }

        public void HookSurvivor(int index)
        {
            survivors[index].SwitchState(survivors[index].stateHooked);
        }

        private void AddTimer(int seconds, int survivorIndex = -1)
        {
            TimerControl timer = new TimerControl(seconds);
            timer.TimerCompleted += Timer_TimerCompleted;

            if (survivorIndex == -1)
            {
                timers[0].Add(timer);
            }
            else
            {
                timers[survivorIndex].Add(timer);
            }

            ArrangeTimers();
            this.Controls.Add(timer);
            timer.Start();
        }

        private void RemoveTimer(int survivorIndex = -1)
        {
            List<TimerControl> timerList = timers[survivorIndex];

            if (survivorIndex == -1)
            {
                timerList = timers[0];
            }

            foreach (var timer in timerList)
            {
                timer.Stop();
                this.Controls.Remove(timer);
            }
            timerList.Clear();
        }

        private bool TimerExists(int survivorIndex = -1)
        {
            bool exists = false;

            List<TimerControl> timerList = timers[survivorIndex];

            if (survivorIndex == -1)
            {
                timerList = timers[0];
            }

            if (timerList.Count > 0)
            {
                exists = true;
            }

            return exists;
        }

        private void Timer_TimerCompleted(object sender, EventArgs e)
        {
            TimerControl timer = (TimerControl)sender;
            timer.TimerCompleted -= Timer_TimerCompleted;
            for (int i = 0; i < 4; i++)
            {
                if (timers[i].Remove(timer))
                {
                    break;
                }
            }
            this.Controls.Remove(timer);
            ArrangeTimers();
        }

        private void ClearAllTimers()
        {
            foreach (var timerList in timers)
            {
                foreach (var timer in timerList)
                {
                    timer.Stop();
                    this.Controls.Remove(timer);
                }
                timerList.Clear();
            }
        }

        private void ArrangeTimers()
        {
            for (int i = 0; i < 4; i++)
            {
                int y = hookStageCounters[i].Location.Y - timerStartYoffset;
                int x = hookStageCounters[i].Location.X - timerStartXoffset;

                foreach (var timer in timers[i])
                {
                    timer.Location = new Point(x, y);
                    y += timer.Height + timerDistanceYoffset;
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }

    public class TimerControl : Control
    {
        private int seconds;
        private System.Windows.Forms.Timer timer;

        public event EventHandler TimerCompleted;

        public TimerControl(int seconds)
        {
            this.seconds = seconds;
            this.AutoSize = true;
            this.Font = new Font("Arial", 12);
            this.ForeColor = Color.Red;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
              ControlStyles.UserPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint, true);

            this.BackColor = Color.Transparent;


            // Set the size of the control based on the text size
            using (Graphics g = this.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(seconds.ToString(), this.Font);
                this.Size = new Size((int)textSize.Width, (int)textSize.Height);
            }

            this.timer = new System.Windows.Forms.Timer();
            this.timer.Interval = 1000; // Timer interval set to 1 second
            this.timer.Tick += Timer_Tick;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Clear with transparent color (ARGB: A=0)
            e.Graphics.Clear(Color.FromArgb(0, 0, 0, 0));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //Draw the timer text on the control
            //e.Graphics.DrawString(seconds.ToString(), this.Font, new SolidBrush(this.ForeColor), Point.Empty);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (Brush textBrush = new SolidBrush(this.ForeColor))
            {
                e.Graphics.DrawString(seconds.ToString(), this.Font, textBrush, Point.Empty);
            }
        }


        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            seconds--;
            Invalidate(); // Redraw the control to update the timer text

            if (seconds <= 0)
            {
                timer.Stop();
                TimerCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
