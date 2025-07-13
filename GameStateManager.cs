using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class GameStateManager
    {
        TransparentOverlayForm form;

        GameStateBase stateCurrent;
        public GameStateLobby stateLobby;
        public GameStatePlaying statePlaying;

        int timerIntervalMs = 16;

        public GameStateManager(TransparentOverlayForm form)
        {
            this.form = form;
            stateLobby = new(this, form);
            statePlaying = new(this, form);
            SwitchState(stateLobby);
            ScreenMonitoring();
        }

        private void ScreenMonitoring()
        {
            System.Windows.Forms.Timer screenMonitorTimer = new();
            screenMonitorTimer.Interval = timerIntervalMs;
            screenMonitorTimer.Tick += (s, e) =>
            {
                Update();
            };
            screenMonitorTimer.Start();
        }

        public void SwitchState(GameStateBase newState)
        {
            stateCurrent = newState;
            stateCurrent.Enter();
        }

        public void Update()
        {
            stateCurrent.Update();
        }
    }
}
