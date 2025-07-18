using DBDtimer;

public class HotKeyActions
{
    private readonly TransparentOverlayForm form;

    public HotKeyActions(TransparentOverlayForm form)
    {
        this.form = form;
    }

    public void ShowSettings()
    {
        PauseApp();
        form.EnableInput(true);

        if (form.menuForm == null || form.menuForm.IsDisposed)
        {
            form.menuForm = new OverlayMenuForm(form);
            form.menuForm.FormClosed += (_, __) => form.EnableInput(false);
        }

        // Get the screen that the cursor is on
        var screen = Screen.FromPoint(Cursor.Position);
        var screenBounds = screen.WorkingArea;

        // Start from cursor position
        Point desiredLocation = Cursor.Position;

        // Clamp the form’s location to the screen bounds
        int maxX = screenBounds.Right - form.menuForm.Width;
        int maxY = screenBounds.Bottom - form.menuForm.Height;

        desiredLocation.X = Math.Max(screenBounds.Left, Math.Min(desiredLocation.X, maxX));
        desiredLocation.Y = Math.Max(screenBounds.Top, Math.Min(desiredLocation.Y, maxY));

        form.menuForm.Location = desiredLocation;
        form.menuForm.Show();
        form.menuForm.BringToFront();
    }


    public void Exit() => Application.Exit();

    public void Restart()
    {
        Application.Restart();
        Environment.Exit(0);
    }

    public void TriggerPause()
    {
        if (form.gameManager.screenMonitorTimer.Enabled)
        {
            PauseApp();
        }
        else
        {
            UnpauseApp();
        }
    }

    public void UnpauseApp()
    {
        if (!form.gameManager.screenMonitorTimer.Enabled)
        {
            form.toastManager.ShowToast("Hook counter unpaused");
            form.gameManager.screenMonitorTimer.Start();
        }
    }

    private void PauseApp()
    {
        if (form.gameManager.screenMonitorTimer.Enabled)
        {
            form.gameManager.screenMonitorTimer.Stop();
            form.overlayRenderer.ClearOverlay();
            form.toastManager.ShowToast("Hook counter paused");
        }
    }

    public void AddHookStage(int index)
    {
        var survivor = form.survivorManager.survivors[index];
        survivor.hookStages = (survivor.hookStages + 1) % 3;
        form.overlayRenderer.DrawOverlay();
    }

    public void ResetHookStages()
    {
        foreach (Survivor survivor in form.survivorManager.survivors)
        {
            survivor.hookStages = 0;
            survivor.SwitchState(survivor.stateUnhooked);
        }
        form.overlayRenderer.DrawOverlay();
    }

    public void TriggerTimerManually(int index)
    {
        form.timerManager.TriggerTimerManually(index);
        form.overlayRenderer.DrawOverlay();
    }

    public void ClearAllTimers()
    {
        form.timerManager.ClearAllTimers();
        form.overlayRenderer.DrawOverlay();
    }
}
