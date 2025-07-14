using Emgu.CV.Flann;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class GameStatePlaying : GameStateBase
    {
        GameStateManager stateManager;
        TransparentOverlayForm form;
        Rectangle continueSearchArea = new(2243, 1482, 168, 59);

        public GameStatePlaying(GameStateManager stateManager, TransparentOverlayForm form)
        {
            this.stateManager = stateManager;
            this.form = form;

            continueSearchArea = form.scaler.ScaleMenu(continueSearchArea);
        }

        public override void Enter()
        {
            //
        }

        public override void Update()
        {
            //Debug.WriteLine(continueSearchArea);
            //Debug.WriteLine("PLAYING");

            if (form.screenChecker.MatchTemplate(form.screenChecker._continueTemplate, continueSearchArea, 0.8))
            {
                stateManager.SwitchState(stateManager.stateLobby);
            }
            else if (form.screenChecker.UIenabled())
            {
                foreach (var survivor in form.survivors)
                {
                    survivor.Update();
                }

                form.DrawOverlay();
            }
        }
    }
}
