using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class GameStateLobby : GameStateBase
    {
        GameStateManager stateManager;
        TransparentOverlayForm form;

        public GameStateLobby(GameStateManager stateManager, TransparentOverlayForm form)
        {
            this.stateManager = stateManager;
            this.form = form;
        }

        public override void Enter()
        {
            foreach (var survivor in form.survivorManager.survivors)
            {
                survivor.hookStages = 0;
                survivor.SwitchState(survivor.stateUnhooked);
                form.timerManager.ClearAllTimers();

                if (form.overlayRenderer != null )
                {
                    form.overlayRenderer.ClearOverlay();
                }
            }
        }

        public override void Update()
        {
            if (form.screenChecker.UIenabled())
            {
                GameStateBase nextState;

                if (stateManager.manualMode)
                {
                    nextState = stateManager.statePlayingManual;
                }
                else
                {
                    nextState = stateManager.statePlaying;
                }

                stateManager.SwitchState(nextState);
            }
        }
    }
}
