using Emgu.CV.Flann;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class SurvivorStateUnhooked : SurvivorStateBase
    {
        int index;
        TransparentOverlayForm form;

        int yOffset = 120;
        Rectangle searchArea = new();

        float hookedThreshold = 0.6f;
        float moriedThreshold = 0.55f;

        public SurvivorStateUnhooked(int index, Rectangle searchArea, TransparentOverlayForm form)
        {
            this.index = index;
            this.yOffset = form.scaler.Scale(this.yOffset);
            int yOffset = index * this.yOffset;

            this.searchArea = new Rectangle(searchArea.X, searchArea.Y + yOffset, searchArea.Width, searchArea.Height);
            this.form = form;
        }

        public override void Enter()
        {
            //
        }

        public override void Update(int index)
        {
            if (form.screenChecker.MatchTemplate(form.screenChecker._hookedTemplate, searchArea, hookedThreshold))
            {
                Debug.WriteLine($"{index} HOOKED");
                form.survivorManager.HookSurvivor(index);
            }
            else if (form.screenChecker.MatchTemplate(form.screenChecker._moriedTemplate, searchArea, moriedThreshold))
            {
                Debug.WriteLine($"{index} MORIED");
                form.survivorManager.KillSurvivor(index);
            }
        }
    }
}
