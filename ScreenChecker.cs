using Emgu.CV.CvEnum;
using Emgu.CV;
using System.Diagnostics;
using System.Drawing.Imaging;
using DBD_Hook_Counter.Properties;

namespace DBD_Hook_Counter
{
    public class ScreenChecker
    {
        TransparentOverlayForm form;

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

        Rectangle uiSearchArea = new(151, 1158, 37, 54);
        Rectangle pauseMenuSearchArea = new(106, 1468, 139, 77);

        public int uiMissingCounter = 0;
        public int uiMissingThreshold = 10;
        public float uiMatchThreshold = 0.6f;
        public float pauseMatchThreshold = 0.8f;

        public ScreenChecker(TransparentOverlayForm form)
        {
            this.form = form;

            _hookedTemplate = form.scaler.LoadScaledTemplate(Resources.hooked);
            _bloodSplatterTemplate = form.scaler.LoadScaledTemplate(Resources.blood_splatter);
            _deadTemplate = form.scaler.LoadScaledTemplate(Resources.dead);
            _moriedTemplate = form.scaler.LoadScaledTemplate(Resources.moried);
            _continueTemplate = form.scaler.LoadScaledTemplate(Resources.continue_button, true);
            _pauseMenuTemplate = form.scaler.LoadScaledTemplate(Resources.back_button, true);
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
            using Mat frame = CaptureScreenMat();        
            using Mat roi = new Mat(frame, region);
            using Mat result = new Mat();

            CvInvoke.MatchTemplate(roi, template, result,
                                   TemplateMatchingType.CcoeffNormed);

            double minVal = 0, maxVal = 0;
            Point minLoc = Point.Empty, maxLoc = Point.Empty;
            CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal,
                               ref minLoc, ref maxLoc);   

            bool match = maxVal >= threshold;

            if (debug && match)
            {
                Debug.WriteLine($"MatchTemplate: {text}");
                SaveImage(frame);
                SaveImage(roi);
                Debug.WriteLine(maxVal);
            }

            return match;
        }

        private static void SaveImage(Mat roi)
        {
            string folder = Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                "DebugCaptures");
            Directory.CreateDirectory(folder);

            string file = Path.Combine(
                folder,
                $"UI_{DateTime.Now:HH_mm_ss_fff}.png");

            roi.Save(file);
        }

        public bool MatchTemplateGrayscale(Mat template,
                                   Rectangle region,
                                   double threshold = 0.90,
                                   bool debug = false,
                                   string text = "")
        {
            using Mat frame = CaptureScreenMat();
            using Mat roiColor = new Mat(frame, region);
            using Mat roi = new();
            using Mat grayTemplate = new();
            using Mat result = new();

            // Convert both ROI and template to grayscale
            CvInvoke.CvtColor(roiColor, roi, ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(template, grayTemplate, ColorConversion.Bgr2Gray);

            // Match using grayscale
            CvInvoke.MatchTemplate(roi, grayTemplate, result, TemplateMatchingType.CcoeffNormed);

            double minVal = 0, maxVal = 0;
            Point minLoc = Point.Empty, maxLoc = Point.Empty;
            CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

            bool match = maxVal >= threshold;

            if (debug && match)
            {
                SaveImage(frame);
                SaveImage(roi);
                Debug.WriteLine(maxVal);
            }

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
            if (MatchTemplateGrayscale(_pauseMenuTemplate, pauseMenuSearchArea, pauseMatchThreshold, debug))
            {
                return false;
            }

            bool hook = MatchTemplateGrayscale(_uiHookTemplate, uiSearchArea, uiMatchThreshold, debug);
            bool mori = MatchTemplateGrayscale(_uiMoriTemplate, uiSearchArea, uiMatchThreshold, debug);

            if (hook || mori)
            {
                form.gameManager.pauseInProgress = false;
                uiMissingCounter = 0;
            }
            else
            {
                form.gameManager.pauseInProgress = true;
                uiMissingCounter++;
            }

            //Debug.WriteLine($"uiMissingCounter: {uiMissingCounter}");
            //Debug.WriteLine($"form.gameManager.pauseInProgress: {form.gameManager.pauseInProgress}");
            bool hudPresent = uiMissingCounter < uiMissingThreshold;  

            return hudPresent;
        }

    }
}
