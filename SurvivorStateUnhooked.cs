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
            if (form.hookManager.MatchTemplate(form.hookManager._hookedTemplate, searchArea))
            {
                form.hookManager.HookSurvivor(index);
            }
            else if (form.hookManager.MatchTemplate(form.hookManager._moriedTemplate, searchArea, 0.55))
            {
                form.hookManager.KillSurvivor(index);
            }
        }
    }
}
