using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProxyBotLib;

namespace StarcraftAI.UnitAI
{
    public interface IUnitAgent
    {
        void Update(ProxyBot bot);
    }
}
