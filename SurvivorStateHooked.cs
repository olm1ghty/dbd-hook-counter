using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    internal class SurvivorStateHooked : SurvivorStateBase
    {
        Color hookPixelColor1 = Color.FromArgb(255, 255, 255);
        Color hookPixelColor2 = Color.FromArgb(255, 0, 0);

        Point hookPixel1 = new();
        Point hookPixel2 = new();
        Point hookPixel3 = new();

        int hookPixel1tolerance = 80;

        private int hookPixel2Rtolerance = 130;
        private int hookPixel2Gtolerance = 75;
        private int hookPixel2Btolerance = 75;

        private int hookPixel3Rtolerance = 150;
        private int hookPixel3Gtolerance = 75;
        private int hookPixel3Btolerance = 75;

        int whiteThreshold = 20;
        int pixelYoffset = 120;

        Rectangle unhookSearchArea = new();
        Rectangle nextStageSearchArea = new();

        public SurvivorStateHooked(int index, Point hookPixel1, Point hookPixel2, Point hookPixel3, Rectangle searchArea, Rectangle nextStageSearchArea)
        {
            int yOffset = index * pixelYoffset;
            this.hookPixel1 = new Point(hookPixel1.X, hookPixel1.Y + yOffset);
            this.hookPixel2 = new Point(hookPixel2.X, hookPixel2.Y + yOffset);
            this.hookPixel3 = new Point(hookPixel3.X, hookPixel3.Y + yOffset);

            this.unhookSearchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
            this.nextStageSearchArea = new Rectangle(nextStageSearchArea.X, nextStageSearchArea.Y + yOffset, nextStageSearchArea.Width, nextStageSearchArea.Height);
        }

        public override void Enter(int index)
        {
            //
        }

        public override void Update(int index, MainForm mainForm)
        {
            if (!mainForm.MatchTemplate(mainForm.hookedTemplate, unhookSearchArea, 0.7))
            {
                mainForm.TriggerTimer(index);
                mainForm.UnhookSurvivor(index);
            }
            else if (mainForm.MatchTemplate(mainForm.nextStageTemplate, nextStageSearchArea, 0.5))
            {
                mainForm.AddHookStage(index);
            }
        }

        public override void Exit()
        {
            //
        }
    }
}
