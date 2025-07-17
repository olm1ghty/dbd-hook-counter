using Emgu.CV;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class OverlayRenderer
    {
        TransparentOverlayForm form;

        Bitmap bmp;
        Graphics graphics;

        public int hookStageCounterStartX;
        public int hookStageCounterStartY;
        public int hookStageCounterOffset;

        SvgDocument hookCounterSVG;

        float hookSVGscaleX;
        float hookSVGscaleY;

        public OverlayRenderer(TransparentOverlayForm form)
        {
            this.form = form;

            hookStageCounterStartX = form.scaler.Scale(GameSettings.hookStageCounterStartX);
            hookStageCounterStartY = form.scaler.ScaleY(GameSettings.hookStageCounterStartY);
            hookStageCounterOffset = form.scaler.Scale(GameSettings.hookStageCounterOffset);

            bmp = new Bitmap(form.Width, form.Height, PixelFormat.Format32bppArgb);
            graphics = Graphics.FromImage(bmp);
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var svgStream = new MemoryStream(Properties.Resources.hooks))
            {
                hookCounterSVG = SvgDocument.Open<SvgDocument>(svgStream);
            }

            float desiredWidth = 20;
            float desiredHeight = 30;
            float svgWidth = hookCounterSVG.Bounds.Width;
            float svgHeight = hookCounterSVG.Bounds.Height;
            hookSVGscaleX = desiredWidth / svgWidth;
            hookSVGscaleY = desiredHeight / svgHeight;

            hookCounterSVG.FillOpacity = 0.80f;
        }

        public void DrawOverlay()
        {
            // wipe everything that was drawn last time ⟵  IMPORTANT
            graphics.Clear(Color.Transparent);

            // --- draw hook stages ---
            for (int i = 0; i < form.survivorManager.survivors.Length; i++)
            {
                hookCounterSVG.Fill = new SvgColourServer(Color.Transparent);

                SvgElement leftHook = hookCounterSVG.GetElementById("leftHook");
                leftHook.Fill = new SvgColourServer(Color.Transparent);

                SvgElement rightHook = hookCounterSVG.GetElementById("rightHook");
                rightHook.Fill = new SvgColourServer(Color.Transparent);

                switch (form.survivorManager.survivors[i].hookStages)
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

                var state = graphics.Save();
                graphics.TranslateTransform(hookStageCounterStartX, (hookStageCounterStartY) + (i * hookStageCounterOffset));
                graphics.ScaleTransform(hookSVGscaleX, hookSVGscaleY);
                hookCounterSVG.Draw(graphics);
                graphics.Restore(state);
            }

            // clean up expired timers before drawing
            form.timerManager.RemoveExpiredTimers();

            // draw all active timers
            foreach (var list in form.timerManager.timers)
            {
                foreach (var timer in list)
                {
                    string txt = timer.SecondsRemaining.ToString();
                    using (Font f = new Font("Arial", 12, System.Drawing.FontStyle.Bold))
                    using (Brush b = new SolidBrush(timer.color))
                    {
                        graphics.DrawString(txt, f, b, timer.Position);
                    }
                }
            }

            // ----- draw toasts -----
            form.toastManager.toasts.RemoveAll(t => !t.IsAlive);          // drop finished ones

            foreach (var toast in form.toastManager.toasts)
            {
                using var f = new Font("Arial", 16, FontStyle.Bold);
                int a = toast.CurrentAlpha;
                using var b = new SolidBrush(Color.FromArgb(a, Color.Yellow));
                graphics.DrawString(toast.Text, f, b, toast.Position);
            }

            NativeMethods.SetBitmapToForm(form, bmp);
        }

        public void ClearOverlay()
        {
            if (graphics != null
                && bmp != null)
            {
                graphics.Clear(Color.Transparent);
                NativeMethods.SetBitmapToForm(form, bmp);
            }
        }
    }
}
