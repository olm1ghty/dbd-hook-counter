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
        public readonly Mat _statusChangeTemplate;
        public readonly Mat _deadTemplate;
        public readonly Mat _moriedTemplate;
        public readonly Mat _continueTemplate;
        public readonly Mat _uiHookTemplate;
        public readonly Mat _uiMoriTemplate;
        public readonly Mat _stbTemplate;

        Rectangle uiSearchArea = new(151, 1158, 33, 50);

        public ScreenChecker(TransparentOverlayForm form)
        {
            this.form = form;

            _hookedTemplate = form.scaler.LoadScaledTemplate(Resources.hooked);
            _statusChangeTemplate = form.scaler.LoadScaledTemplate(Resources.status_change);
            _deadTemplate = form.scaler.LoadScaledTemplate(Resources.dead);
            _moriedTemplate = form.scaler.LoadScaledTemplate(Resources.moried);
            _continueTemplate = form.scaler.LoadScaledTemplateMenu(Resources.continue_button);
            _uiHookTemplate = form.scaler.LoadScaledTemplate(Resources.ui_hook);
            _uiMoriTemplate = form.scaler.LoadScaledTemplate(Resources.ui_mori);
            _stbTemplate = form.scaler.LoadScaledTemplate(Resources.stb);

            uiSearchArea = form.scaler.Scale(uiSearchArea);
        }

        public bool MatchTemplate(Mat template, Rectangle region, double threshold = 0.90)
        {
            // 1. Grab the screen once, directly into a Mat
            using Mat frame = CaptureScreenMat();

            // 2. Crop to the region of interest (no extra copy)
            using Mat roi = new Mat(frame, region);

            // 3. Run template‑matching
            using Mat result = new Mat();
            CvInvoke.MatchTemplate(roi, template, result, TemplateMatchingType.CcoeffNormed);

            // 4. Find the best match
            double minVal = 0, maxVal = 0;
            Point minLoc = Point.Empty, maxLoc = Point.Empty;
            CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

            return maxVal >= threshold;
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

        public bool UIenabled()
        {
            bool enabled = false;

            if (MatchTemplate(_uiHookTemplate, uiSearchArea, 0.5)
                ||
                MatchTemplate(_uiMoriTemplate, uiSearchArea, 0.5))
            {
                enabled = true;
            }

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
