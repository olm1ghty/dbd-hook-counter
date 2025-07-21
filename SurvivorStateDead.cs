namespace DBD_Hook_Counter
{
    public class SurvivorStateDead : SurvivorStateBase
    {
        TransparentOverlayForm form;
        Survivor survivor;

        public SurvivorStateDead(TransparentOverlayForm form, Survivor survivor)
        {
            this.form = form;
            this.survivor = survivor;
        }

        public override void Enter()
        {
            form.timerManager.RemoveTimer(survivor.index);
            survivor.hookStages = 2;
        }

        public override void Update(int index)
        {
            //
        }
    }
}
