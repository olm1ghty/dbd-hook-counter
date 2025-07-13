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

            _hookedTemplate = LoadTemplate(Resources.hooked, form.aspectRatioMod);
            _statusChangeTemplate = LoadTemplate(Resources.status_change, form.aspectRatioMod);
            _deadTemplate = LoadTemplate(Resources.dead, form.aspectRatioMod);
            _moriedTemplate = LoadTemplate(Resources.moried, form.aspectRatioMod);
            _continueTemplate = LoadTemplate(Resources.continue_button, form.aspectRatioMod);
            _uiHookTemplate = LoadTemplate(Resources.ui_hook, form.aspectRatioMod);
            _uiMoriTemplate = LoadTemplate(Resources.ui_mori, form.aspectRatioMod);
            _stbTemplate = LoadTemplate(Resources.stb, form.aspectRatioMod);

            uiSearchArea = new(
                (int)(uiSearchArea.X * form.aspectRatioMod),
                (int)((uiSearchArea.Y + form.blackBorderMod) * form.aspectRatioMod),
                (int)(uiSearchArea.Width * form.aspectRatioMod),
                (int)(uiSearchArea.Height * form.aspectRatioMod));
        }

        Mat LoadTemplate(Bitmap bmp, double scale)
        {
            Mat full = bmp.ToMat();                        // original size
            if (Math.Abs(scale - 1.0) < 0.0001) return full;

            Mat small = new Mat();
            CvInvoke.Resize(full, small, Size.Empty,       // Size.Empty → use fx/fy
                            scale, scale, Inter.Area);     // good for shrinking
            full.Dispose();                                // if you don't need it
            return small;
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
