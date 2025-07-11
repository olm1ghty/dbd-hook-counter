using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;

namespace DBDtimer
{
    public class HookManager
    {
        public Image<Bgr, byte> hookedTemplate = new Image<Bgr, byte>(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\States\Hooked.png");
        public Image<Bgr, byte> nextStageTemplate = new Image<Bgr, byte>(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\States\next_stage.png");

        TransparentOverlayForm form;

        private System.Timers.Timer screenMonitorTimer;
        private const int checkIntervalMs = 1000;
        private Point safetyPixel1 = new Point(170, 1180);
        private Point safetyPixel2 = new Point(235, 1284);
        private Color safetyPixel1Color = Color.White;
        private Color safetyPixel2Color = Color.Black;
        private int safetyPixel1tolerance = 150;
        private int safetyPixel2tolerance = 100;
        int whiteThreshold = 20;

        public HookManager(TransparentOverlayForm form)
        {
            this.form = form;
            StartHookDetection();
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

        public static Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            return bitmap;
        }

        public void AddHookStage(int survivorIndex)
        {
            Survivor survivor = form.survivors[survivorIndex];

            if (survivor.hookStages < 2)
            {
                survivor.hookStages++;
            }
            else
            {
                survivor.hookStages = 0;
            }

            form.timerManager.RemoveTimer(survivorIndex);
        }

        public void HookSurvivor(int index)
        {
            form.survivors[index].SwitchState(form.survivors[index].stateHooked);
        }

        public void UnhookSurvivor(int index)
        {
            form.survivors[index].SwitchState(form.survivors[index].stateUnhooked);
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
                //if (UIenabled())
                {
                    foreach (var survivor in form.survivors)
                    {
                        form.Invoke(new Action(() =>
                        {
                            survivor.Update();
                            form.DrawOverlay();
                        }));
                    }
                }
            }
            catch { }
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
    }
}
