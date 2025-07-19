using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBD_Hook_Counter
{
    public abstract class GameStateBase
    {
        public abstract void Enter();
        public abstract void Update();
    }
}
