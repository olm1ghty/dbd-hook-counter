namespace DBD_Hook_Counter
{
    public class Survivor
    {
        public SurvivorStateUnhooked stateUnhooked;
        public SurvivorStateHooked stateHooked;
        public SurvivorStateDead stateDead;
        public SurvivorStateIntermediate stateIntermediate;
        public SurvivorStateBase currentState;

        Rectangle statusSearchArea;
        Rectangle bloodSplatterSearchArea;
        Rectangle stbSearchArea;

        TransparentOverlayForm form;

        public int index = 0;
        public int hookStages = 0;

        public Survivor(int index, TransparentOverlayForm form)
        {
            this.index = index;
            this.form = form;

            statusSearchArea = form.scaler.Scale(GameSettings.statusSearchArea);
            bloodSplatterSearchArea = form.scaler.Scale(GameSettings.bloodSplatterSearchArea);
            stbSearchArea = form.scaler.Scale(GameSettings.stbSearchArea);

            stateUnhooked = new(index, statusSearchArea, form);
            stateHooked = new(index, statusSearchArea, bloodSplatterSearchArea, stbSearchArea, form, this);
            stateDead = new(form, this);
            stateIntermediate = new();

            SwitchStateFromIntermediate(stateUnhooked);
        }

        public void SwitchState(SurvivorStateBase newState)
        {
            currentState = stateIntermediate;
            currentState.Enter();
            stateIntermediate.SwitchToNextState(newState, this);
        }

        public void SwitchStateFromIntermediate(SurvivorStateBase newState)
        {
            currentState = newState;
            currentState.Enter();
        }

        public void Update()
        {
            currentState.Update(index);
        }

        public void GetHooked()
        {
            form.timerManager.RemoveTimer(index);

            switch (hookStages)
            {
                case 0:
                    SwitchState(stateHooked);
                    hookStages = 1;
                    break;

                case 1:
                    SwitchState(stateHooked);
                    hookStages = 2;
                    break;

                case 2:
                    SwitchState(stateDead);
                    break;
            }
        }
    }
}
