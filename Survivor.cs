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
        public SurvivorStateUnhooked stateUnhooked;
        public SurvivorStateHooked stateHooked;
        public SurvivorStateDead stateDead;
        public SurvivorStateIntermediate stateIntermediate;
        public SurvivorStateBase currentState;

        Rectangle statusSearchArea = new(140, 632, 64, 84);
        Rectangle bloodSplatterSearchArea = new(211, 622, 27, 70);
        Rectangle stbSearchArea = new(299, 653, 10, 28);
        Rectangle progressBarSearchArea = new(250, 711, 60, 13);

        TransparentOverlayForm form;

        public int index = 0;
        public int hookStages = 0;

        public Survivor(int index, TransparentOverlayForm form)
        {
            this.index = index;
            this.form = form;

            statusSearchArea = form.scaler.Scale(statusSearchArea);
            bloodSplatterSearchArea = form.scaler.Scale(bloodSplatterSearchArea);
            stbSearchArea = form.scaler.Scale(stbSearchArea);
            progressBarSearchArea = form.scaler.Scale(progressBarSearchArea);

            stateUnhooked = new(index, statusSearchArea, form);
            stateHooked = new(index, statusSearchArea, bloodSplatterSearchArea, stbSearchArea, progressBarSearchArea, form, this);
            stateDead = new(form, this);
            stateIntermediate = new();

            SwitchStateFromIntermediate(stateUnhooked);
        }

        public void SwitchState(SurvivorStateBase newState)
        {
            currentState = stateIntermediate;
            currentState.Enter();
            stateIntermediate.SwitchToNextState(newState, this);
        }

        public void SwitchStateFromIntermediate(SurvivorStateBase newState)
        {
            currentState = newState;
            currentState.Enter();
        }

        public void Update()
        {
            currentState.Update(index);
            //Debug.WriteLine($"Survivor: {currentState}.Update({index}, mainForm)");
            //Debug.WriteLine($"Hook stages: {hookStages}");
        }

        public void GetHooked()
        {
            form.timerManager.RemoveTimer(index);

            switch (hookStages)
            {
                case 0:
                    SwitchState(stateHooked);
                    hookStages = 1;
                    break;

                case 1:
                    SwitchState(stateHooked);
                    hookStages = 2;
                    break;

                case 2:
                    SwitchState(stateDead);
                    break;
            }
        }
    }
}
