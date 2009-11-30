using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProxyBotLib;
using ProxyBotLib.Data;
using ProxyBotLib.Types;
using StarcraftAI.UnitAI;

namespace StarcraftAI.CommanderAI
{
    public class Commander
    {
        private Unit center;
        private List<Unit> units;
        private List<Unit> buildings;
        private List<int> currentlyBuilding;

        public Commander()
        {
            units = new List<Unit>();
            buildings = new List<Unit>();
            currentlyBuilding = new List<int>();
        }

        public void Update(ProxyBot bot, Dictionary<int, IUnitAgent> agents)
        {
            //Track units
            updateUnits(bot);

            //Assign the center if we haven't done so
            if (center == null)
            {
                center = (from b in buildings
                          where b.Type.Center
                          select b).Single();
            }

            //If we have no barracks, build one
            buildBarracksIfNeeded(bot, agents);
            
            //If we have a barracks, and can affort marines, build some
            buildMarines(bot);
        }
        public void FinishedBuilding(int typeID)
        {
            Console.Out.WriteLine("[Commander] Finished building");
            currentlyBuilding.Remove(typeID);
        }

        private void buildMarines(ProxyBot bot)
        {
            if (bot.Player.SupplyUsed < bot.Player.SupplyTotal &&
                bot.Player.Minerals > bot.unitTypes[Constants.Terran_Marine].MineralsCost + 100 &&
                !currentlyBuilding.Contains(Constants.Terran_Marine))
            {
                int barracksCount = (from b in buildings
                                     where b.Type.ID == Constants.Terran_Barracks
                                     select b).Count();
                if (barracksCount > 0 && 
                    !currentlyBuilding.Contains(Constants.Terran_Barracks))
                {
                    int barracksID = (from b in buildings
                                      where b.Type.ID == Constants.Terran_Barracks
                                      select b.ID).Single();
                    currentlyBuilding.Add(Constants.Terran_Marine);
                    bot.train(barracksID, Constants.Terran_Marine);
                }
            }
        }

        private void buildBarracksIfNeeded(ProxyBot bot, Dictionary<int, IUnitAgent> agents)
        {
            var barracks = (from b in buildings
                            where b.Type.ID == Constants.Terran_Barracks &&
                            b.PlayerID == bot.PlayerID
                            select b).ToList();
            if (barracks.Count == 0 &&
                !currentlyBuilding.Contains(Constants.Terran_Barracks))
            {
                //Check if we have enough resources for a barracks
                if (bot.Player.Race == Constants.Terran)
                {
                    if (bot.Player.Minerals > bot.unitTypes[Constants.Terran_Barracks].MineralsCost)
                    {
                        //Find the nearest minerals and build 15 tiles toward
                        //the centre
                        Unit nearestResource = null;
                        foreach (Unit u in bot.Units)
                        {
                            if (u.Type.ID != Constants.Resource_Mineral_Field)
                            {
                                continue;
                            }
                            if (nearestResource == null)
                            {
                                nearestResource = u;
                            }
                            else
                            {
                                if (center.distance(u) < center.distance(nearestResource))
                                {
                                    nearestResource = u;
                                }
                            }
                        }

                        int x = 0;
                        int y = center.Y;

                        if (center.X < bot.Map.MapWidth / 2)
                        {
                            x = nearestResource.X + 15;
                        }
                        else
                        {
                            x = nearestResource.X - 15;
                        }

                        if (bot.Map.isBuildable(x, y))
                        {
                            Unit u = getNearestWorker();
                            Worker workerAI = agents[u.ID] as Worker;
                            Console.Out.WriteLine("[Commander] Building barracks");
                            workerAI.Build(Constants.Terran_Barracks, x, y);
                            currentlyBuilding.Add(Constants.Terran_Barracks);
                        }
                    }
                }
            }
        }

        private void updateUnits(ProxyBot bot)
        {
            //Reset the units
            units = new List<Unit>();
            buildings = new List<Unit>();

            foreach (Unit u in bot.Units)
            {
                if (u.PlayerID == bot.PlayerID)
                {
                    if (!u.Type.Building &&
                        u.Type.ID != Constants.Resource_Mineral_Field &&
                        u.Type.ID != Constants.Resource_Vespene_Geyser)
                    {
                        units.Add(u);
                    }

                    if (u.Type.Building)
                    {
                        buildings.Add(u);
                    }
                }
            }
        }

        private Unit getNearestWorker()
        {
            //Get the worker nearest to the command centre
            units.Sort((Unit u1, Unit u2) =>
            {
                double d1 = u1.distance(center);
                double d2 = u2.distance(center);
                return d1.CompareTo(d2);
            });
            return (from u in units
                    where u.Type.Worker
                    select u).First();
        }
    }
}
