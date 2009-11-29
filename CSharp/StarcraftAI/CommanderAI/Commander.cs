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
        private Dictionary<int, Unit> units;
        private Dictionary<int, Unit> buildings;
        private List<int> currentlyBuilding;

        public Commander()
        {
            units = new Dictionary<int, Unit>();
            buildings = new Dictionary<int, Unit>();
            currentlyBuilding = new List<int>();
        }

        public void Update(ProxyBot bot, Dictionary<int, IUnitAgent> agents)
        {
            //Track units
            foreach(Unit u in bot.Units)
            {
                if (!units.Keys.Contains(u.ID))
                {
                    units.Add(u.ID, u);
                }
                else
                {
                    units[u.ID] = u;
                }
            }

            //Assign the center if we haven't done so
            if (center == null)
            {
                center = (from u in units.Values
                          where u.Type.Center && u.PlayerID == bot.PlayerID
                          select u).Single();
            }

            //TODO: Work on this, it's dodgy as
            var barracks = (from b in buildings.Values
                            where b.Type.ID == Constants.Terran_Barracks &&
                            b.PlayerID == bot.PlayerID
                            select b).ToList();
            if (barracks.Count == 0 && !currentlyBuilding.Contains(Constants.Terran_Barracks))
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
                            workerAI.Build(Constants.Terran_Barracks, x, y);
                            currentlyBuilding.Add(Constants.Terran_Barracks);
                        }
                    }
                }
            }
        }

        private Unit getNearestWorker()
        {
            //Get the worker nearest to the command centre
            List<Unit> sortableUnits = units.Values.ToList();
            sortableUnits.Sort((Unit u1, Unit u2) =>
            {
                double d1 = u1.distance(center);
                double d2 = u2.distance(center);
                return d1.CompareTo(d2);
            });
            return (from u in sortableUnits
                    where u.Type.Worker
                    select units[u.ID]).First();
        }
    }
}
