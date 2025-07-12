using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class SurvivorStateUnhooked : SurvivorStateBase
    {
        //Color lastPixelColor1 = Color.Empty;
        //Color lastPixelColor2 = Color.Empty;

        //Point hookPixel1 = new();
        //Point hookPixel2 = new();
        //Point hookPixel3 = new();

        //int hookPixel1tolerance = 130;

        //private int hookPixel2Rtolerance = 130;
        //private int hookPixel2Gtolerance = 75;
        //private int hookPixel2Btolerance = 75;

        //private int hookPixel3Rtolerance = 150;
        //private int hookPixel3Gtolerance = 100;
        //private int hookPixel3Btolerance = 100;

        //int whiteThreshold = 20;
        //int redExpressiveness = 20;

        TransparentOverlayForm form;

        int yOffset = 120;
        Rectangle searchArea = new();

        public SurvivorStateUnhooked(int index, Rectangle searchArea, TransparentOverlayForm form)
        {
            int yOffset = index * this.yOffset;
            //this.hookPixel1 = new Point(hookPixel1.X, hookPixel1.Y + yOffset);
            //this.hookPixel2 = new Point(hookPixel2.X, hookPixel2.Y + yOffset);
            //this.hookPixel3 = new Point(hookPixel3.X, hookPixel3.Y + yOffset);

            this.searchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
            this.form = form;
        }

        public override void Update(int index)
        {
            if (form.hookManager.MatchTemplate(form.hookManager.deadTemplate, searchArea))
            {
                form.hookManager.KillSurvivor(index);
            }
            else if (form.hookManager.MatchTemplate(form.hookManager.hookedTemplate, searchArea))
            {
                form.hookManager.HookSurvivor(index);
                form.hookManager.AddHookStage(index);
            }
        }
    }
}
