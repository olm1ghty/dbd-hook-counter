using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    abstract public class SurvivorStateBase
    {
        public abstract void Enter();
        public abstract void Update(int index);
    }
}
