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
        TransparentOverlayForm form;

        int yOffset = 120;
        Rectangle searchArea = new();

        public SurvivorStateUnhooked(int index, Rectangle searchArea, TransparentOverlayForm form)
        {
            this.yOffset = form.scaler.Scale(this.yOffset);
            int yOffset = index * this.yOffset;

            this.searchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
            this.form = form;
        }

        public override void Enter()
        {
            //
        }

        public override void Update(int index)
        {
            if (form.screenChecker.MatchTemplate(form.screenChecker._hookedTemplate, searchArea, 0.65))
            {
                form.survivorManager.HookSurvivor(index);
            }
            else if (form.screenChecker.MatchTemplate(form.screenChecker._moriedTemplate, searchArea, 0.55))
            {
                form.survivorManager.KillSurvivor(index);
            }
        }
    }
}
