using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class SurvivorStateHooked : SurvivorStateBase
    {
        //Color hookPixelColor1 = Color.FromArgb(255, 255, 255);
        //Color hookPixelColor2 = Color.FromArgb(255, 0, 0);

        //Point hookPixel1 = new();
        //Point hookPixel2 = new();
        //Point hookPixel3 = new();

        //int hookPixel1tolerance = 80;

        //private int hookPixel2Rtolerance = 130;
        //private int hookPixel2Gtolerance = 75;
        //private int hookPixel2Btolerance = 75;

        //private int hookPixel3Rtolerance = 150;
        //private int hookPixel3Gtolerance = 75;
        //private int hookPixel3Btolerance = 75;

        //int whiteThreshold = 20;
        int yOffset = 120;

        Rectangle unhookSearchArea = new();
        Rectangle nextStageSearchArea = new();

        TransparentOverlayForm form;

        public SurvivorStateHooked(int index, Rectangle searchArea, Rectangle nextStageSearchArea, TransparentOverlayForm form)
        {
            int yOffset = index * this.yOffset;
            //this.hookPixel1 = new Point(hookPixel1.X, hookPixel1.Y + yOffset);
            //this.hookPixel2 = new Point(hookPixel2.X, hookPixel2.Y + yOffset);
            //this.hookPixel3 = new Point(hookPixel3.X, hookPixel3.Y + yOffset);

            this.unhookSearchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
            this.nextStageSearchArea = new Rectangle(nextStageSearchArea.X, nextStageSearchArea.Y + yOffset, nextStageSearchArea.Width, nextStageSearchArea.Height);
            this.form = form;
        }

        public override void Update(int index)
        {
            if (!form.hookManager.MatchTemplate(form.hookManager.hookedTemplate, unhookSearchArea, 0.7))
            {
                form.timerManager.TriggerTimer(index);
                form.hookManager.UnhookSurvivor(index);
            }
            else if (form.hookManager.MatchTemplate(form.hookManager.nextStageTemplate, nextStageSearchArea, 0.5))
            {
                form.hookManager.AddHookStage(index);
            }
        }
    }
}
