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
    }
}