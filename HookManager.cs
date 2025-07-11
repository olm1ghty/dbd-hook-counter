using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class HookManager
    {
        public Image<Bgr, byte> hookedTemplate = new Image<Bgr, byte>(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\States\Hooked.png");
        public Image<Bgr, byte> nextStageTemplate = new Image<Bgr, byte>(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\States\next_stage.png");

        TransparentOverlayForm form;

        public HookManager(TransparentOverlayForm form)
        {
            this.form = form;
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
    }
}
