using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBD_Hook_Counter
{
    public class SurvivorStateHooked : SurvivorStateBase
    {
        int index;

        Rectangle statusSearchArea = new();
        Rectangle bloodSplatterSearchArea = new();
        Rectangle stbSearchArea = new();
        Rectangle progressBarSearchArea = new();

        TransparentOverlayForm form;
        Survivor survivor;

        float deadThreshold = 0.5f;
        float unhookThreshold = 0.65f;
        float bloodSplatterThreshold = 0.7f;
        float stbThreshold = 0.5f;

        public SurvivorStateHooked(int index, Rectangle searchArea, Rectangle nextStageSearchArea, Rectangle stbSearchArea, Rectangle unhookSearchArea, TransparentOverlayForm form, Survivor survivor)
        {
            this.index = index;
            int yOffset = form.scaler.Scale(GameSettings.hookStageCounterOffset);
            yOffset = index * yOffset;

            this.statusSearchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
            this.bloodSplatterSearchArea = new Rectangle(nextStageSearchArea.X, nextStageSearchArea.Y + yOffset, nextStageSearchArea.Width, nextStageSearchArea.Height);
            this.stbSearchArea = new Rectangle(stbSearchArea.X, stbSearchArea.Y + yOffset, stbSearchArea.Width, stbSearchArea.Height);
            this.progressBarSearchArea = new Rectangle(unhookSearchArea.X, unhookSearchArea.Y + yOffset, unhookSearchArea.Width, unhookSearchArea.Height);

            this.form = form;
            this.survivor = survivor;
        }

        public override void Enter()
        {
            form.timerManager.RemoveTimer(survivor.index);
        }

        public override void Update(int index)
        {
            if (SurvivorDead())
            {
                Debug.WriteLine($"{index} DEAD");
                form.survivorManager.KillSurvivor(index);
            }
            else if (SurvivorUnhooked())
            {
                Debug.WriteLine($"{index} UNHOOKED");
                CheckForSTB();

                form.timerManager.TriggerTimer(index);
                form.survivorManager.UnhookSurvivor(index);
            }
            else if (BloodSplatter())
            {
                Debug.WriteLine($"{index} ADDITIONAL HOOK STAGE");
                form.survivorManager.HookSurvivor(index);
            }
        }

        private bool BloodSplatter()
        {
            return form.screenChecker.MatchTemplate(form.screenChecker._bloodSplatterTemplate, bloodSplatterSearchArea, bloodSplatterThreshold, text: "blood");
        }

        private bool SurvivorDead()
        {
            return form.screenChecker.MatchTemplate(form.screenChecker._deadTemplate, statusSearchArea, deadThreshold, text: "dead");
        }

        private bool SurvivorUnhooked()
        {
            return ( !form.gameManager.pauseInProgress && 
                !form.screenChecker.MatchTemplate(form.screenChecker._hookedTemplate, statusSearchArea, unhookThreshold, text: "hook"));
        }

        private void CheckForSTB()
        {
            foreach (var survivor in form.survivorManager.survivors)
            {
                if (survivor.currentState == survivor.stateUnhooked)
                {
                    if (form.screenChecker.MatchTemplate(form.screenChecker._stbTemplate, survivor.stateHooked.stbSearchArea, stbThreshold, text: "stb")
                        && form.screenChecker.MatchTemplate(form.screenChecker._bloodSplatterTemplate, survivor.stateHooked.bloodSplatterSearchArea, bloodSplatterThreshold, text: "blood"))
                    {
                        this.survivor.hookStages--;
                    }
                }
            }
        }
    }
}
