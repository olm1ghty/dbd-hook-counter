using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    internal class SurvivorStateUnhooked : SurvivorStateBase
    {
        Color lastPixelColor1 = Color.Empty;
        Color lastPixelColor2 = Color.Empty;

        Point hookPixel1 = new();
        Point hookPixel2 = new();
        Point hookPixel3 = new();
        int pixelYoffset = 120;

        int hookPixel1tolerance = 130;

        private int hookPixel2Rtolerance = 130;
        private int hookPixel2Gtolerance = 75;
        private int hookPixel2Btolerance = 75;

        private int hookPixel3Rtolerance = 150;
        private int hookPixel3Gtolerance = 100;
        private int hookPixel3Btolerance = 100;

        int whiteThreshold = 20;
        int redExpressiveness = 20;

        Rectangle searchArea = new();

        public SurvivorStateUnhooked(int index, Point hookPixel1, Point hookPixel2, Point hookPixel3, Rectangle searchArea)
        {
            int yOffset = index * pixelYoffset;
            this.hookPixel1 = new Point(hookPixel1.X, hookPixel1.Y + yOffset);
            this.hookPixel2 = new Point(hookPixel2.X, hookPixel2.Y + yOffset);
            this.hookPixel3 = new Point(hookPixel3.X, hookPixel3.Y + yOffset);

            this.searchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
        }

        public override void Enter(int index)
        {
            //
        }

        public override void Update(int index, MainForm mainForm)
        {
            if (mainForm.MatchTemplate(mainForm.hookedTemplate, searchArea))
            {
                mainForm.AddHookStage(index);
                mainForm.HookSurvivor(index);
            }
        }

        public override void Exit()
        {
            //
        }
    }
}
