using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class SurvivorManager
    {
        TransparentOverlayForm form;

        public SurvivorManager(TransparentOverlayForm form)
        { 
            this.form = form;
        }

        public void HookSurvivor(int index)
        {
            form.survivors[index].GetHooked();
        }

        public void KillSurvivor(int index)
        {
            var survivor = form.survivors[index];
            survivor.SwitchState(survivor.stateDead);
            form.timerManager.RemoveTimer(index);
        }

        public void UnhookSurvivor(int index)
        {
            form.survivors[index].SwitchState(form.survivors[index].stateUnhooked);
        }

        public void AddHookStage(int survivorIndex)
        {
            //Debug.WriteLine($"AddHookStage: {survivorIndex}");

            form.survivors[survivorIndex].hookStages++;
        }
    }
}
