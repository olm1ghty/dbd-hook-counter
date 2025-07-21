using System.Diagnostics;

namespace DBD_Hook_Counter
{
    public class SurvivorStateHooked : SurvivorStateBase
    {
        int index;

        Rectangle statusSearchArea = new();
        Rectangle bloodSplatterSearchArea = new();
        Rectangle stbSearchArea = new();

        TransparentOverlayForm form;
        Survivor survivor;

        float deadThreshold = 0.5f;
        float unhookThreshold = 0.5f;
        float bloodSplatterThreshold = 0.75f;
        float stbThreshold = 0.5f;

        public SurvivorStateHooked(int index, Rectangle statusSearchArea, Rectangle bloodSplatterSearchArea, Rectangle stbSearchArea, TransparentOverlayForm form, Survivor survivor)
        {
            this.index = index;
            int yOffset = form.scaler.ScaleOffsetX(GameSettings.hookStageCounterOffset);
            yOffset = index * yOffset;

            this.statusSearchArea = new Rectangle(statusSearchArea.X, statusSearchArea.Y + yOffset, statusSearchArea.Width, statusSearchArea.Height);
            this.bloodSplatterSearchArea = new Rectangle(bloodSplatterSearchArea.X, bloodSplatterSearchArea.Y + yOffset, bloodSplatterSearchArea.Width, bloodSplatterSearchArea.Height);
            this.stbSearchArea = new Rectangle(stbSearchArea.X, stbSearchArea.Y + yOffset, stbSearchArea.Width, stbSearchArea.Height);

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
                //Debug.WriteLine($"{index} DEAD");
                form.survivorManager.KillSurvivor(index);
            }
            else if (SurvivorUnhooked())
            {
                //Debug.WriteLine($"{index} UNHOOKED");
                CheckForSTB();

                form.timerManager.TriggerTimer(index);
                form.survivorManager.UnhookSurvivor(index);
            }
            else if (BloodSplatter())
            {
                //Debug.WriteLine($"{index} ADDITIONAL HOOK STAGE");
                form.survivorManager.HookSurvivor(index);
            }
        }

        private bool BloodSplatter()
        {
            return form.screenChecker.MatchTemplate(form.screenChecker._bloodSplatterTemplate, bloodSplatterSearchArea, bloodSplatterThreshold, text: "blood", debug:false);
        }

        private bool SurvivorDead()
        {
            return form.screenChecker.MatchTemplateGrayscale(form.screenChecker._deadTemplate, statusSearchArea, deadThreshold, text: "dead");
        }

        private bool SurvivorUnhooked()
        {
            return (!form.gameManager.pauseInProgress && 
                !form.screenChecker.MatchTemplateGrayscale(form.screenChecker._hookedTemplate, statusSearchArea, unhookThreshold, text: "hook", debug:false));
        }

        private void CheckForSTB()
        {
            foreach (var survivor in form.survivorManager.survivors)
            {
                if (survivor.currentState == survivor.stateUnhooked)
                {
                    if (form.screenChecker.MatchTemplateGrayscale(form.screenChecker._stbTemplate, survivor.stateHooked.stbSearchArea, stbThreshold, text: "stb")
                        && form.screenChecker.MatchTemplate(form.screenChecker._bloodSplatterTemplate, survivor.stateHooked.bloodSplatterSearchArea, bloodSplatterThreshold, text: "blood"))
                    {
                        this.survivor.hookStages--;
                    }
                }
            }
        }
    }
}
