using System;
//UPGRADE_TODO: The type 'starcraftbot.proxybot.Constants.Order' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using Order = ProxyBotLib.Types.Constants.Order;
using ProxyBotLib.Agent;
using ProxyBotLib;
using ProxyBotLib.Types;
using ProxyBotLib.Data;
using System.ComponentModel;
using System.Windows.Forms;

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

            //Fire up influence map
            InfluenceMap = new InfluenceMap(proxyBot);
            float[,] currentMap = null;
			
			while (true)
			{
				try
				{
					System.Threading.Thread.Sleep(new System.TimeSpan((System.Int64) 10000 * 250));
				}
				catch (System.Exception)
				{
				}

                if (currentMap == null)
                {
                    currentMap = InfluenceMap.GetMap(InfluenceMap.Terrain.GROUND, 50);
                }
				
				foreach(Unit unit in proxyBot.Units)
				{
					//Send all units off to death!
                    int x = 0;
                    int y = 0;
                    foreach (Unit u in proxyBot.Units)
                    {
                        if (u.PlayerID != playerID && u.Type.Center)
                        {
                            x = u.X;
                            y = u.Y;
                            break;
                        }
                    }
                    if (unit.PlayerID == playerID && unit.Type.Worker &&
                        unit.Order != (int)Order.Move)
                    {
                        proxyBot.rightClick(unit.ID, x, y);
                    }
                    /*
                    //Experiment with influence map for navigation
                    var ux = unit.X;
                    var uy = unit.Y;
                    float highestInfluence = 0f;
                    int highestTileX = 0;
                    int highestTileY = 0;
                    //Look 10 tiles around
                    for (var w = ux - 10; w <= ux + 10; w++)
                    {
                        for (var h = uy - 10; h <= uy + 10; h++)
                        {
                            if (w < 0 || w >= currentMap.GetLength(0) ||
                                h < 0 || h >= currentMap.GetLength(1))
                            {
                                //Projected tile is off the screen
                                continue;
                            }
                            if (currentMap[w, h] > highestInfluence)
                            {
                                highestTileX = w;
                                highestTileY = h;
                                highestInfluence = currentMap[w, h];
                            }

                        }
                    }
                    if (highestTileX != 0 && highestTileY != 0 && 
                        unit.PlayerID == playerID && unit.Type.Worker &&
                        unit.Order != (int)Order.Move)
                    {
                        proxyBot.rightClick(unit.ID, highestTileX, highestTileY);
                    }
                    */

					// make idle works mine	
			        /*
					if (unit.PlayerID == playerID && unit.Type.Worker)
					{
						
						if (unit.Order == Convert.ToInt32(Order.PlayerGuard))
						{
							int closestID = - 1;
							//UPGRADE_TODO: The equivalent in .NET for field 'java.lang.Double.MAX_VALUE' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
							double closest = System.Double.MaxValue;
							
							foreach(Unit patch in proxyBot.Units)
							{
								
								if (patch.Type.ID == Constants.Resource_Mineral_Field)
								{
									
									double distance = (0.5 + new Random().NextDouble()) * unit.distance(patch);
									if (distance < closest)
									{
										closest = distance;
										closestID = patch.ID;
									}
								}
							}
							
							if (closestID != - 1)
							{
								//System.Console.Out.WriteLine("Right on patch: " + unit.ID + " " + closestID);
								proxyBot.rightClick(unit.ID, closestID);
							}
						}
					}
                    */
				}
			}
		}
	}
}