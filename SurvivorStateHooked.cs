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
        int yOffset = 120;

        Rectangle unhookSearchArea = new();
        Rectangle nextStageSearchArea = new();

        TransparentOverlayForm form;
        Survivor survivor;

        public SurvivorStateHooked(int index, Rectangle searchArea, Rectangle nextStageSearchArea, TransparentOverlayForm form, Survivor survivor)
        {
            int yOffset = index * this.yOffset;

            this.unhookSearchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
            this.nextStageSearchArea = new Rectangle(nextStageSearchArea.X, nextStageSearchArea.Y + yOffset, nextStageSearchArea.Width, nextStageSearchArea.Height);
            this.form = form;
            this.survivor = survivor;
        }

        public override void Enter()
        {
            form.timerManager.RemoveTimer(survivor.index);
        }

        public override void Update(int index)
        {
            if (form.screenChecker.MatchTemplate(form.screenChecker._deadTemplate, unhookSearchArea, 0.5))
            {
                form.screenChecker.KillSurvivor(index);
            }
            else if (!form.screenChecker.MatchTemplate(form.screenChecker._hookedTemplate, unhookSearchArea, 0.7))
            {
                form.timerManager.TriggerTimer(index);
                form.screenChecker.UnhookSurvivor(index);
            }
            else if (form.screenChecker.MatchTemplate(form.screenChecker._nextStageTemplate, nextStageSearchArea, 0.4))
            {
                form.screenChecker.HookSurvivor(index);
            }
        }
    }
}
