namespace DBD_Hook_Counter
{
    public class SurvivorManager
    {
        TransparentOverlayForm form;

        public Survivor[] survivors = new Survivor[4];

        public SurvivorManager(TransparentOverlayForm form)
        { 
            this.form = form;

            survivors = new Survivor[]
            {
            new Survivor(0, form),
            new Survivor(1, form),
            new Survivor(2, form),
            new Survivor(3, form)
            };
        }

        public void HookSurvivor(int index)
        {
            survivors[index].GetHooked();
        }

        public void KillSurvivor(int index)
        {
            var survivor = survivors[index];
            survivor.SwitchState(survivor.stateDead);
            form.timerManager.RemoveTimer(index);
        }

        public void UnhookSurvivor(int index)
        {
            survivors[index].SwitchState(survivors[index].stateUnhooked);
        }

        public void AddHookStage(int survivorIndex)
        {
            survivors[survivorIndex].hookStages++;
        }
    }
}
