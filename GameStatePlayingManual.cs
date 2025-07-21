namespace DBD_Hook_Counter
{
    public class GameStatePlayingManual : GameStateBase
    {
        GameStateManager stateManager;
        TransparentOverlayForm form;
        Rectangle continueSearchArea = GameSettings.continueSearchArea;
        float continueThreshold = 0.7f;

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
            if (form.screenChecker.MatchTemplate(form.screenChecker._continueTemplate, continueSearchArea, continueThreshold, debug: false))
            {
                stateManager.SwitchState(stateManager.stateLobby);
            }
            else if (form.screenChecker.UIenabled())
            {
                form.overlayRenderer.DrawOverlay();
            }
            else
            {
                form.overlayRenderer.ClearOverlay();
            }
        }
    }
}
