using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProxyBotLib.Data;
using ProxyBotLib.Types;
using StarcraftAI.CommanderAI;

namespace StarcraftAI.UnitAI
{
    public class AgentFactory
    {
        public static IUnitAgent AttachAgent(Unit u, Commander c)
        {
            IUnitAgent agent = null;
            if (u.Type.Worker)
            {
                agent = new Worker(u, c);
            }
            return agent;
        }
    }
}
