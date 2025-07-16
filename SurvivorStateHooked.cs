using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class SurvivorStateHooked : SurvivorStateBase
    {
        int yOffset = 120;

        Rectangle statusSearchArea = new();
        Rectangle statusChangeSearchArea = new();
        Rectangle stbSearchArea = new();

        TransparentOverlayForm form;
        Survivor survivor;

        float deadThreshold = 0.5f;
        float unhookThreshold = 0.45f;
        float nextStageThreshold = 0.5f;
        float stbThreshold = 0.5f;

        public SurvivorStateHooked(int index, Rectangle searchArea, Rectangle nextStageSearchArea, Rectangle stbSearchArea, TransparentOverlayForm form, Survivor survivor)
        {
            this.yOffset = form.scaler.Scale(this.yOffset);
            int yOffset = index * this.yOffset;

            this.statusSearchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
            this.statusChangeSearchArea = new Rectangle(nextStageSearchArea.X, nextStageSearchArea.Y + yOffset, nextStageSearchArea.Width, nextStageSearchArea.Height);
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
            if (form.screenChecker.MatchTemplate(form.screenChecker._deadTemplate, statusSearchArea, deadThreshold))
            {
                form.survivorManager.KillSurvivor(index);
            }
            else if (!form.gameManager.pauseInProgress &&
                !form.screenChecker.MatchTemplate(form.screenChecker._hookedTemplate, statusSearchArea, unhookThreshold, debug: false))
            {
                // check for STB
                foreach (var survivor in form.survivors)
                {
                    // check every unhooked survivor
                    if (survivor.currentState == survivor.stateUnhooked)
                    {
                        if (form.screenChecker.MatchTemplate(form.screenChecker._stbTemplate, survivor.stateHooked.stbSearchArea, stbThreshold)
                            && form.screenChecker.MatchTemplate(form.screenChecker._statusChangeTemplate, survivor.stateHooked.statusChangeSearchArea, 0.4))
                        {
                            this.survivor.hookStages--;
                        }
                    }
                }

                form.timerManager.TriggerTimer(index);
                form.survivorManager.UnhookSurvivor(index);
            }
            else if (form.screenChecker.MatchTemplate(form.screenChecker._statusChangeTemplate, statusChangeSearchArea, nextStageThreshold))
            {
                form.survivorManager.HookSurvivor(index);
            }
        }
    }
}
