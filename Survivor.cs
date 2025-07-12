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
        SurvivorStateBase currentState;

        Rectangle hookSearchArea = new(123, 630, 90, 90);
        Rectangle nextStageSearchArea = new(187, 608, 100, 70);

        TransparentOverlayForm form;

        public int index = 0;
        public int hookStages = 0;

        public Survivor(int index, TransparentOverlayForm form)
        {
            this.index = index;
            this.form = form;

            stateUnhooked = new(index, hookSearchArea, form);
            stateHooked = new(index, hookSearchArea, nextStageSearchArea, form, this);
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
