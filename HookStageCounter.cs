using Svg;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace DBDtimer
{
    public class HookStageCounter : Control
    {
        private int hookStages = 0;

        public int HookStages
        {
            get { return hookStages; }
            set
            {
                hookStages = value;
                Invalidate(); // Redraw control when hook stages change
            }
        }

<<<<<<< HEAD

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            int circleDiameter = 20;
            int spacing = 10;
            int startX = 10;

            for (int i = 0; i < hookStages; i++)
            {
                int x = startX + i * (circleDiameter + spacing);
                int y = (Height - circleDiameter) / 2; // Center vertically

                g.FillEllipse(Brushes.Red, x, y, circleDiameter, circleDiameter);
            }
        }
=======
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);

        //    var svg = SvgDocument.Open(@"C:\Users\user\Desktop\Other development\DBDtimer\dbd-hook-counter\resources\both hooks.svg");
        //    svg.Width = this.Width;
        //    svg.Height = this.Height;

        //    // Create a transparent bitmap
        //    using (var bmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb))
        //    using (var g = Graphics.FromImage(bmp))
        //    {
        //        g.Clear(Color.Transparent); // ensures real alpha
        //        svg.Draw(g); // render directly to transparent surface

        //        e.Graphics.CompositingMode = CompositingMode.SourceOver;
        //        e.Graphics.DrawImage(bmp, 0, 0);
        //    }
        //}
>>>>>>> in-progress
    }
}