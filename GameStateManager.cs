using ExCSS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace DBDtimer
{
    public class GameStateManager
    {
        TransparentOverlayForm form;

        GameStateBase stateCurrent;
        public GameStateLobby stateLobby;
        public GameStatePlaying statePlaying;

        public System.Windows.Forms.Timer screenMonitorTimer = new();
        int timerIntervalMs = 16;

        System.Timers.Timer temporaryPauseTimer = new();
        int temporaryPauseDuration = 1000;

        // global flag
        public volatile bool pauseInProgress = false;

        public GameStateManager(TransparentOverlayForm form)
        {
            this.form = form;
            stateLobby = new(this, form);
            statePlaying = new(this, form);
            SwitchState(stateLobby);
            InitializeScreenMonitoring();
        }

        public void TemporaryPause()
        {
            temporaryPauseTimer.Stop();
            temporaryPauseTimer.Dispose();
            pauseInProgress = true;
            form.Invoke((Action)(() =>
            {
                screenMonitorTimer.Stop();
                form.overlayRenderer.ClearOverlay();
            }));
            form.screenChecker.uiMissingCounter = form.screenChecker.uiMissingThreshold;

            temporaryPauseTimer = new();
            temporaryPauseTimer.Interval = temporaryPauseDuration;
            temporaryPauseTimer.Elapsed += (s, e) =>
            {
                form.BeginInvoke(() =>
                {
                    screenMonitorTimer.Start();
                    pauseInProgress = false;
                });
                temporaryPauseTimer.Stop();
            };
            temporaryPauseTimer.Start();
        }

        private void InitializeScreenMonitoring()
        {
            screenMonitorTimer = new();
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
