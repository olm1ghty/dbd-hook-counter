using Svg;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace DBD_Hook_Counter
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

            hookStageCounterStartX = form.scaler.ScaleOffsetX(GameSettings.hookStageCounterStartX);
            hookStageCounterStartY = form.scaler.ScaleYFromBottomAnchor(GameSettings.hookStageCounterStartY);
            hookStageCounterOffset = form.scaler.ScaleYOffsetHUD(GameSettings.hookStageCounterOffset);

            bmp = new Bitmap(form.Width, form.Height, PixelFormat.Format32bppArgb);
            graphics = Graphics.FromImage(bmp);
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var svgStream = new MemoryStream(Properties.Resources.hooks))
            {
                hookCounterSVG = SvgDocument.Open<SvgDocument>(svgStream);
            }

            float desiredWidth = 20 * form.scaler.resolutionScaleY;
            float desiredHeight = 30 * form.scaler.resolutionScaleX;
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

                var svgBounds = hookCounterSVG.Bounds;

                // Anchor to center
                float anchorX = svgBounds.Width / 2f;
                float anchorY = svgBounds.Height / 2f;

                var state = graphics.Save();
                graphics.TranslateTransform(
                    hookStageCounterStartX,
                    hookStageCounterStartY + (i * hookStageCounterOffset)
                );
                graphics.ScaleTransform(hookSVGscaleX, hookSVGscaleY);

                // Shift origin back to center of the SVG
                graphics.TranslateTransform(-anchorX, -anchorY);

                hookCounterSVG.Draw(graphics);
                graphics.Restore(state);
            }

            form.timerManager.RemoveExpiredTimers();

            // draw all active timers
            foreach (var list in form.timerManager.timers)
            {
                foreach (var timer in list)
                {
                    string txt = timer.SecondsRemaining.ToString();
                    using (Font f = new Font("Segoe UI", 12, System.Drawing.FontStyle.Bold))
                    using (Brush b = new SolidBrush(timer.color))
                    {
                        graphics.DrawString(txt, f, b, timer.Position);
                    }
                }
            }

            // ----- draw toasts -----
            form.toastManager.toasts.RemoveAll(t => !t.IsAlive);

            foreach (var toast in form.toastManager.toasts)
            {
                using var f = new Font("Segoe UI", 16, FontStyle.Bold);
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
