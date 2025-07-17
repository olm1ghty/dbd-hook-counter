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
        form.EnableInput(true);

        if (form.menuForm == null || form.menuForm.IsDisposed)
        {
            form.menuForm = new OverlayMenuForm(form);
            form.menuForm.FormClosed += (_, __) => form.EnableInput(false);
        }

        form.menuForm.Location = Cursor.Position;
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
            form.gameManager.screenMonitorTimer.Stop();
            form.overlayRenderer.ClearOverlay();
            form.toastManager.ShowToast("App paused");
        }
        else
        {
            form.toastManager.ShowToast("App unpaused");
            form.gameManager.screenMonitorTimer.Start();
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
