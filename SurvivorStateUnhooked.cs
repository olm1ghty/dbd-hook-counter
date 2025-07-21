using System.Diagnostics;

namespace DBD_Hook_Counter
{
    public class SurvivorStateUnhooked : SurvivorStateBase
    {
        int index;
        TransparentOverlayForm form;

        Rectangle searchArea = new();

        float hookedThreshold = 0.6f;
        float moriedThreshold = 0.55f;

        public SurvivorStateUnhooked(int index, Rectangle searchArea, TransparentOverlayForm form)
        {
            this.index = index;
            int yOffset = form.scaler.ScaleYOffsetHUD(GameSettings.hookStageCounterOffset);
            yOffset = index * yOffset;

            this.searchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
            this.form = form;
        }

        public override void Enter()
        {
            //
        }

        public override void Update(int index)
        {
            if (form.screenChecker.MatchTemplateGrayscale(form.screenChecker._hookedTemplate, searchArea, hookedThreshold, debug: false))
            {
                //Debug.WriteLine($"{index} HOOKED");
                form.survivorManager.HookSurvivor(index);
            }
            else if (form.screenChecker.MatchTemplateGrayscale(form.screenChecker._moriedTemplate, searchArea, moriedThreshold))
            {
                //Debug.WriteLine($"{index} MORIED");
                form.survivorManager.KillSurvivor(index);
            }
        }
    }
}
