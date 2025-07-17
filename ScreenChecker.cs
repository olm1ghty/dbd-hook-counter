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
using System.Drawing.Imaging;
using DBDtimer.Properties;
using Emgu.CV.Flann;

namespace DBDtimer
{
    public class ScreenChecker
    {
        TransparentOverlayForm form;

        // loaded once
        public readonly Mat _hookedTemplate;
        public readonly Mat _bloodSplatterTemplate;
        public readonly Mat _deadTemplate;
        public readonly Mat _moriedTemplate;
        public readonly Mat _continueTemplate;
        public readonly Mat _pauseMenuTemplate;
        public readonly Mat _uiHookTemplate;
        public readonly Mat _uiMoriTemplate;
        public readonly Mat _stbTemplate;
        public readonly Mat _progressBarTemplate;

        Rectangle uiSearchArea = new(151, 1158, 33, 50);
        Rectangle pauseMenuSearchArea = new(116, 1478, 119, 57);

        public int uiMissingCounter = 0;
        public int uiMissingThreshold = 5;

        private int uiSeenCounter = 0;
        private int uiSeenThreshold = 5;

        public ScreenChecker(TransparentOverlayForm form)
        {
            this.form = form;

            _hookedTemplate = form.scaler.LoadScaledTemplate(Resources.hooked);
            _bloodSplatterTemplate = form.scaler.LoadScaledTemplate(Resources.blood_splatter);
            _deadTemplate = form.scaler.LoadScaledTemplate(Resources.dead);
            _moriedTemplate = form.scaler.LoadScaledTemplate(Resources.moried);
            _continueTemplate = form.scaler.LoadScaledTemplateMenu(Resources.continue_button);
            _pauseMenuTemplate = form.scaler.LoadScaledTemplateMenu(Resources.back_button);
            _uiHookTemplate = form.scaler.LoadScaledTemplate(Resources.ui_hook);
            _uiMoriTemplate = form.scaler.LoadScaledTemplate(Resources.ui_mori);
            _stbTemplate = form.scaler.LoadScaledTemplate(Resources.stb);
            _progressBarTemplate = form.scaler.LoadScaledTemplate(Resources.progress_bar);

            uiSearchArea = form.scaler.Scale(uiSearchArea);
            pauseMenuSearchArea = form.scaler.ScaleMenu(pauseMenuSearchArea);
        }

        public bool MatchTemplate(Mat template,
                          Rectangle region,
                          double threshold = 0.90,
                          bool debug = false,
                          string text = "")
        {
            //Debug.WriteLine($"MatchTemplate: {text}");

            using Mat frame = CaptureScreenMat();        // fresh frame each call
            using Mat roi = new Mat(frame, region);
            using Mat result = new Mat();

            CvInvoke.MatchTemplate(roi, template, result,
                                   TemplateMatchingType.CcoeffNormed);

            double minVal = 0, maxVal = 0;
            Point minLoc = Point.Empty, maxLoc = Point.Empty;
            CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal,
                               ref minLoc, ref maxLoc);   // ← unchanged

            bool match = maxVal >= threshold;

            // ---------- NEW: save only the matching frame ----------
            if (debug && match)
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "DebugCaptures");
                Directory.CreateDirectory(folder);

                string file = Path.Combine(
                    folder,
                    $"UI_{DateTime.Now:HH_mm_ss_fff}.png");

                frame.Save(file);
            }
            // --------------------------------------------------------

            return match;
        }

        Mat CaptureScreenMat()
        {
            Rectangle roi = Screen.PrimaryScreen.Bounds;
            using Bitmap bmp = new Bitmap(roi.Width, roi.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, roi.Size);
            }

            return bmp.ToMat();
        }

        public bool UIenabled(bool debug = false)
        {
            if (MatchTemplate(_pauseMenuTemplate, pauseMenuSearchArea, 0.8, debug))
            {
                uiSeenCounter = 0;
                return false;
            }

            bool hook = MatchTemplate(_uiHookTemplate, uiSearchArea, 0.8, debug);
            bool mori = MatchTemplate(_uiMoriTemplate, uiSearchArea, 0.8, debug);

            if (hook || mori)
            {
                form.gameManager.pauseInProgress = false;
                uiMissingCounter = 0;
                //uiSeenCounter++;        
            }
            else
            {
                form.gameManager.pauseInProgress = true;
                uiMissingCounter++;
                //uiSeenCounter = 0;        
            }

            //Debug.WriteLine($"uiMissingCounter: {uiMissingCounter}");
            //Debug.WriteLine($"form.gameManager.pauseInProgress: {form.gameManager.pauseInProgress}");
            //Debug.WriteLine($"uiSeenStreak: {uiSeenCounter}");
            bool hudPresent = uiMissingCounter < uiMissingThreshold
                           /*&& uiSeenStreak >= uiSeenThreshold*/;  

            return hudPresent;
        }

    }
}
