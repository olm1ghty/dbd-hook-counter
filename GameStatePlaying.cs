using Emgu.CV.Flann;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBD_Hook_Counter
{
    public class GameStatePlaying : GameStateBase
    {
        GameStateManager stateManager;
        TransparentOverlayForm form;
        Rectangle continueSearchArea = GameSettings.continueSearchArea;

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
            if (form.screenChecker.MatchTemplate(form.screenChecker._continueTemplate, continueSearchArea, 0.8))
            {
                stateManager.SwitchState(stateManager.stateLobby);
            }
            else if (form.screenChecker.UIenabled())
            {
                foreach (var survivor in form.survivorManager.survivors)
                {
                    survivor.Update();
                }

                form.overlayRenderer.DrawOverlay();
            }
            else
            {
                form.overlayRenderer.ClearOverlay();
            }
        }
    }
}
