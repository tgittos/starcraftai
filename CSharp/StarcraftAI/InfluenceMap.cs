using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProxyBotLib.Data;
using ProxyBotLib;

namespace StarcraftAI
{
    public class InfluenceMap
    {
        public enum Terrain
        {
            GROUND,
            AIR
        }
        private float[,] groundInfluence;
        private float[,] flyingInfluence;
        private ProxyBot data;

        public InfluenceMap(ProxyBot pData)
        {
            data = pData;
            groundInfluence = new float[data.Map.MapWidth, data.Map.MapHeight];
            flyingInfluence = new float[data.Map.MapWidth, data.Map.MapHeight];
            populateGroundInfluence();
        }


        public float[,] GetMap(Terrain t)
        {
            switch (t)
            {
                case Terrain.GROUND:
                    return groundInfluence;
                case Terrain.AIR:
                    return flyingInfluence;
            }
            return groundInfluence;
        }
        public float[,] GetMap(Terrain t, int CentreInfluence)
        {
            //Return the map with the enemies base factored in at the influence level
            float[,] terrainMap = GetMap(t);
            foreach (Unit u in data.Units)
            {
                if (u.PlayerID != data.PlayerID && u.Type.Center)
                {
                    terrainMap = SetInterestPoint(terrainMap, CentreInfluence, u.X, u.Y);
                }
            }
            return terrainMap;
        }
        
        public static float[,] SetInterestPoint(float[,] influence, int weight, int x, int y)
        {
            //Determine radius of a circle with it's centre at x,y
            //big enough to encompass the play map
            int dx = (x < influence.GetLength(0) / 2) ? influence.GetLength(0) - x : x;
            int dy = (y < influence.GetLength(1) / 2) ? influence.GetLength(1) - y : y;
            double r = Math.Sqrt(dx * dx + dy * dy);

            //Go through each tile, and assign a value
            //The value is calculated by determining how far away from the circle
            //centre the tile is, and finding it's ratio to the radius. Further tiles
            //have a higher ratio.
            //Multiply this ratio by the influence inverted, so that further away tiles
            //get less influence
            for (var lx = 0; lx < influence.GetLength(0); lx++)
            {
                for (var ly = 0; ly < influence.GetLength(1); ly++)
                {
                    //If the tile is walkable
                    if (influence[lx, ly] > 0)
                    {
                        int tdx = Math.Abs(x - lx);
                        int tdy = Math.Abs(y - ly);
                        double tr = Math.Sqrt(tdx * tdx + tdy * tdy);
                        float tileInfluence = weight - ((float)(tr / r) * (float)weight);
                        influence[lx, ly] += tileInfluence;
                    }
                }
            }
            return influence;
        }

        private void populateGroundInfluence()
        {
            for (var x = 0; x < groundInfluence.GetLength(0); x++)
            {
                for (var y = 0; y < groundInfluence.GetLength(1); y++)
                {
                    /*
                    if (data.Map.isWalkable(x, y))
                    {
                        groundInfluence[x, y] = 5f;
                    }
                    else
                    */
                    groundInfluence[x, y] = 5f;
                    if (!data.Map.isWalkable(x, y))
                    {
                        //Impassible terrain has no influence
                        //And also exerts a negative influence around it
                        for (var nx = x - 4; nx <= x + 4; nx++)
                        {
                            for (var ny = y - 4; ny <= y + 4; ny++)
                            {
                                if (nx < 0 || nx >= groundInfluence.GetLength(0) ||
                                    ny < 0 || ny >= groundInfluence.GetLength(1))
                                {
                                    //Out of bounds
                                    continue;
                                }
                                var dnx = Math.Abs(x - nx);
                                var dny = Math.Abs(y - ny);
                                var r = Math.Sqrt(dnx * dnx + dny + dny);
                                double dist = r / 4 * 4;
                                groundInfluence[x, y] -= (float)dist;
                            }
                        }
                    }
                }
            }
        }
    }
}
