using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    internal class Survivor
    {
        Point hookPixel1 = new Point(173, 669);
        Point hookPixel2 = new Point(159, 693);
        Point hookPixel3 = new Point(256, 716);

        public SurvivorStateUnhooked stateUnhooked;
        public SurvivorStateHooked stateHooked;
        public SurvivorStateDead stateDead;
        SurvivorStateBase currentState;

        Rectangle hookSearchArea = new(123, 630, 90, 90);
        Rectangle nextStageSearchArea = new(187, 608, 100, 70);

        MainForm mainForm;

        int index = 0;
        public int hookStages = 0;

        public Survivor(int index, MainForm mainForm)
        {
            this.index = index;
            this.mainForm = mainForm;

            stateUnhooked = new(index, hookPixel1, hookPixel2, hookPixel3, hookSearchArea);
            stateHooked = new(index, hookPixel1, hookPixel2, hookPixel3, hookSearchArea, nextStageSearchArea);
            stateDead = new();

            SwitchState(stateUnhooked);
        }

        public void SwitchState(SurvivorStateBase newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }
            
            currentState = newState;
            newState.Enter(index);
        }

        public void Update()
        {
            currentState.Update(index, mainForm);
            //Console.WriteLine($"Survivor: {currentState}.Update({index}, mainForm)");
        }
    }
}
