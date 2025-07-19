using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBD_Hook_Counter
{
    public class GameStatePlayingManual : GameStateBase
    {
        GameStateManager stateManager;
        TransparentOverlayForm form;
        Rectangle continueSearchArea = GameSettings.continueSearchArea;

        public GameStatePlayingManual(GameStateManager stateManager, TransparentOverlayForm form)
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
        }
    }
}
