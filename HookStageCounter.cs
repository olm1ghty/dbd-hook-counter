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
    }
}