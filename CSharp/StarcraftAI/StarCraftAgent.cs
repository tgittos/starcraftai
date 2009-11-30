using System;
//UPGRADE_TODO: The type 'starcraftbot.proxybot.Constants.Order' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using Order = ProxyBotLib.Types.Constants.Order;
using ProxyBotLib.Agent;
using ProxyBotLib;
using ProxyBotLib.Types;
using ProxyBotLib.Data;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using StarcraftAI.UnitAI;
using System.Linq;
using StarcraftAI.CommanderAI;

namespace StarcraftAI
{
	/// <summary> Throw in your bot code here.</summary>
	public class StarCraftAgent : IAgent
	{
        public InfluenceMap InfluenceMap { get; set; }

        private ProxyBot proxyBot;

        public StarCraftAgent()
        {
        }

        public virtual void Start(ProxyBot pProxy)
		{
            this.proxyBot = pProxy;

			int playerID = proxyBot.PlayerID;

            //AI List
            Dictionary<int, IUnitAgent> agents = new Dictionary<int, IUnitAgent>();

            //Create the commander
            Commander commander = new Commander();
			
			while (true)
			{
				try
				{
					//System.Threading.Thread.Sleep(new System.TimeSpan((System.Int64) 10000 * 250));
                    System.Threading.Thread.Sleep(new System.TimeSpan((System.Int64)10000 * 500));
				}
				catch (System.Exception)
				{
				}
				
				foreach(Unit unit in proxyBot.Units)
				{
                    //Attach an AI to any new unit
                    foreach (Unit u in proxyBot.Units)
                    {
                        if (u.PlayerID == playerID)
                        {
                            if (!agents.Keys.Contains(u.ID))
                            {
                                agents.Add(u.ID, AgentFactory.AttachAgent(u, commander));
                            }
                        }
                    }

                    //Update from command centre
                    commander.Update(proxyBot, agents);

                    //Update the unit agents
                    foreach (KeyValuePair<int, IUnitAgent> kvp in agents)
                    {
                        if (kvp.Value != null)
                        {
                            kvp.Value.Update(proxyBot);
                        }
                    }
				}
			}
		}
	}
}