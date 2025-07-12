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
    }
}