using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    abstract class SurvivorStateBase
    {
        public abstract void Enter(int index);
        public abstract void Update(int index, MainForm mainForm);
        public abstract void Exit();
    }
}
