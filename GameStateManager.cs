using DBD_Hook_Counter.Properties;

namespace DBD_Hook_Counter
{
    public class GameStateManager
    {
        TransparentOverlayForm form;

        GameStateBase stateCurrent;
        public GameStateLobby stateLobby;
        public GameStatePlaying statePlaying;
        public GameStatePlayingManual statePlayingManual;

        public System.Windows.Forms.Timer screenMonitorTimer = new();
        int timerIntervalMs = 16;

        System.Timers.Timer temporaryPauseTimer = new();
        int temporaryPauseDuration = 3000;

        public volatile bool pauseInProgress = false;
        public bool manualMode = false;

        public GameStateManager(TransparentOverlayForm form)
        {
            manualMode = Settings.Default.manualMode;
            this.form = form;

            stateLobby = new(this, form);
            statePlaying = new(this, form);
            statePlayingManual = new(this, form);
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
