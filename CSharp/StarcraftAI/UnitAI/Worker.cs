using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProxyBotLib.Data;
using ProxyBotLib;
using Order = ProxyBotLib.Types.Constants.Order;
using System.Drawing;
using ProxyBotLib.Types;

namespace StarcraftAI.UnitAI
{
    public class Worker : IUnitAgent
    {
        private enum State
        {
            IDLE,
            MINING,
            BUILDING,
            REPAIRING,
            MOVING
        }
        private int unitID;
        private static Dictionary<int, int> unitResources;
        private Unit unit;
        private State currentState;
        private ProxyBot bot;
        private Point destination;
        private int buildingType;
        private int buildingCount;

        public Worker(Unit u)
        {
            unitID = u.ID;
            unit = u;
            unitResources = new Dictionary<int, int>();
            currentState = State.IDLE;
        }

        public void Update(ProxyBot pBot)
        {
            //Update data
            bot = pBot;
            int updatedCount = 0;
            foreach (Unit u in bot.Units)
            {
                if (u.ID == unitID)
                {
                    unit = u;
                }
                if (buildingType > 0 && u.Type.ID == buildingType)
                {
                    updatedCount++;
                }
            }

            checkIfFinishedBuilding(updatedCount);

            if (currentState == State.IDLE)
            {
                //Figure out what to do, considering where the unit is
                //what's going on in the game, and what units are near it
                //For now, hard code it to mine
                Console.Out.WriteLine("[Worker] Mining");
                Mine();
            }
            else if (currentState == State.MOVING)
            {
                //If we're moving, we're on our way to build something.
                updateStateForBuilding();
            }
        }

        public void Build(int unitType, int x, int y)
        {
            destination = new Point(x, y);
            int currentCount = 0;
            foreach (Unit u in bot.Units)
            {
                if (unit.Type.ID == unitType)
                    currentCount++;
            }
            buildingType = unitType;
            buildingCount = currentCount;
            Console.Out.WriteLine("[Worker] Told to build " + unitType + " at " + x + ", " + y);
            updateStateForBuilding();
        }
        public void Mine()
        {
            currentState = State.MINING;

            List<Unit> resources = new List<Unit>();
            foreach (Unit u in bot.Units)
            {
                if (u.Type.ID == Constants.Resource_Mineral_Field/* ||
                    u.Type.ID == Constants.Resource_Vespene_Geyser*/)
                {
                    resources.Add(u);
                }
            }
            resources.Sort(sortUnitByDistance);
            //If the unit is a Vespene Gas gyser, we need to see if there
            //is a vespene gas collector
            foreach (Unit resource in resources)
            {
                if (unitResources.Keys.Contains(resource.ID))
                {
                    //This resource is being mined by a unit
                    continue;
                }
                else if (resource.Type.ID == Constants.Resource_Vespene_Geyser)
                {
                    //TODO: Do this
                    mineGas(resource);
                    break;
                }
                else
                {
                    mineMinerals(resource);
                    break;
                }
            }
        }
        public void Mine(Unit resource)
        {
            currentState = State.MINING;
            mineMinerals(resource);
        }

        private void updateStateForBuilding()
        {
            int dx = Math.Abs(destination.X - unit.X);
            int dy = Math.Abs(destination.Y - unit.Y);
            double d = Math.Sqrt(dx * dx + dy * dy);
            bool closeEnough = d < 5;

            if (currentState != State.BUILDING && closeEnough)
            {
                currentState = State.BUILDING;
                bot.build(unit.ID, destination.X, destination.Y, buildingType);
            }
            else if (currentState != State.MOVING && currentState != State.BUILDING)
            {
                currentState = State.MOVING;
                bot.rightClick(unit.ID, destination.X, destination.Y);
            }
        }
        private void mineMinerals(Unit resource)
        {
            bot.rightClick(unit.ID, resource.ID);
            unitResources.Add(resource.ID, unit.ID);
        }
        private void mineGas(Unit resource)
        {
            if (bot.Player.Minerals > bot.unitTypes[Constants.Terran_Refinery].MineralsCost)
            {
                unitResources.Add(resource.ID, unit.ID);
                bot.build(unit.ID, resource.X, resource.Y, Constants.Terran_Refinery);
            }
            else
            {
                List<Unit> resources = new List<Unit>();
                foreach (Unit u in bot.Units)
                {
                    if (u.Type.ID == Constants.Resource_Mineral_Field)
                    {
                        resources.Add(u);
                    }
                }
                resources.Sort(sortUnitByDistance);
                foreach (Unit r in resources)
                {
                    if (unitResources.Keys.Contains(r.ID))
                    {
                        //This resource is being mined by a unit
                        continue;
                    }
                    else
                    {
                        mineMinerals(r);
                        break;
                    }
                }
            }
        }
        private int sortUnitByDistance(Unit u1, Unit u2)
        {
            double distanceToU1 = u1.distance(unit);
            double distanceToU2 = u2.distance(unit);
            if (distanceToU1 < distanceToU2)
            {
                return -1;
            }
            else if (distanceToU2 < distanceToU1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        private void checkIfFinishedBuilding(int updatedCount)
        {
            if (updatedCount - buildingCount > 0 &&
                unit.Order == (int)Order.PlayerGuard)
            {
                Console.Out.WriteLine("[Worker] Finished building");
                buildingType = 0;
                buildingCount = 0;
                currentState = State.IDLE;
            }
        }
    }
}
