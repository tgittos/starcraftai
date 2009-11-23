using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProxyBotLib.Data;

namespace StarcraftAI
{
    internal class InfluenceMap
    {
        public int[,] Influence
        {
            get
            {
                return baseInfluence;
            }
        }

        private int[,] baseInfluence;
        private int[,] buildingInfluence;

        public InfluenceMap(MapData data)
        {
            baseInfluence = new int[data.MapWidth, data.MapHeight];
            buildingInfluence = new int[data.MapWidth, data.MapHeight];
            fillBaseInfluence(data);
            fillBuildingInfluence(data);
        }

        public static int[,] SetInterestPoint(int[,] influence, int weight, int x, int y)
        {
            /*
            influence[x, y] += weight;
            
            int weightCount = 1;
            while (weightCount <= weight)
            {
                int delta = weight - weightCount;
                for (var lx = x - delta; lx <= x + delta; lx++)
                {
                    for (var ly = y - delta; ly <= y + delta; ly++)
                    {
                        if (lx < 0 || lx > influence.GetLength(0) ||
                            ly < 0 || ly > influence.GetLength(1))
                        {
                            continue;
                        }

                        if (influence[lx, ly] > 0)
                        {
                            influence[lx, ly] += delta;
                        }
                    }
                }
                weightCount++;
            }
            return influence;
            */
            return influence;
        }

        private void fillBaseInfluence(MapData data)
        {
            for (var x = 0; x < baseInfluence.GetLength(0); x++)
            {
                for (var y = 0; y < baseInfluence.GetLength(1); y++)
                {
                    if (data.isWalkable(x, y))
                    {
                        baseInfluence[x, y] = 1;
                    }
                    else
                    {
                        baseInfluence[x, y] = 0;
                    }
                }
            }
        }
        private void fillBuildingInfluence(MapData data)
        {
            //for (var x = 0; x < buildingInfluence
        }
    }
}
