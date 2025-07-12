using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class SurvivorStateDead : SurvivorStateBase
    {
        TransparentOverlayForm form;
        Survivor survivor;

        public SurvivorStateDead(TransparentOverlayForm form, Survivor survivor)
        {
            this.form = form;
            this.survivor = survivor;
        }

        public override void Enter()
        {
            form.timerManager.RemoveTimer(survivor.index);
        }

        public override void Update(int index)
        {
            //
        }
    }
}
