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
            foreach (var survivor in form.survivors)
            {
                survivor.hookStages = 0;
                survivor.SwitchState(survivor.stateUnhooked);
                form.timerManager.ClearAllTimers();
                form.ClearOverlay();
            }
        }

        public override void Update()
        {
            //Debug.WriteLine("LOBBY");

            if (form.screenChecker.UIenabled())
            {
                stateManager.SwitchState(stateManager.statePlaying);
            }
        }
    }
}
