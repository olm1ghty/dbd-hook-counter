using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class Survivor
    {
        //Point hookPixel1 = new Point(173, 669);
        //Point hookPixel2 = new Point(159, 693);
        //Point hookPixel3 = new Point(256, 716);

        public SurvivorStateUnhooked stateUnhooked;
        public SurvivorStateHooked stateHooked;
        public SurvivorStateDead stateDead;
        SurvivorStateBase currentState;

        Rectangle hookSearchArea = new(123, 630, 90, 90);
        Rectangle nextStageSearchArea = new(187, 608, 100, 70);

        TransparentOverlayForm form;

        int index = 0;
        public int hookStages = 0;

        public Survivor(int index, TransparentOverlayForm form)
        {
            this.index = index;
            this.form = form;

            stateUnhooked = new(index, hookSearchArea, form);
            stateHooked = new(index, hookSearchArea, nextStageSearchArea, form);
            stateDead = new();

            SwitchState(stateUnhooked);
        }

        public void SwitchState(SurvivorStateBase newState)
        {
            currentState = newState;
        }

        public void Update()
        {
            currentState.Update(index);
            //Debug.WriteLine($"Survivor: {currentState}.Update({index}, mainForm)");
        }
    }
}
