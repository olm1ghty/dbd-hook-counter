using Timer = System.Windows.Forms.Timer;

namespace DBD_Hook_Counter
{
    public class SurvivorStateIntermediate : SurvivorStateBase
    {
        SurvivorStateBase nextState;
        Survivor survivor;

        public override void Enter()
        {
            //
        }

        public void SwitchToNextState(SurvivorStateBase nextState, Survivor survivor)
        {
            this.nextState = nextState;
            this.survivor = survivor;

            Timer timer = new();
            timer.Interval = 1000;
            timer.Tick += (s, e) => {
                survivor.SwitchStateFromIntermediate(nextState);
                timer.Stop();
            };
            timer.Start();
        }

        public override void Update(int index)
        {
            //
        }
    }
}
