﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ProxyBotLib;
using ProxyBotLib.Data;
using ProxyBotLib.Types;

namespace StarcraftAI.GUI
{
    internal partial class InfluenceMapGUI : Form
    {
        public struct HeatPoint
        {
            public int X;
            public int Y;
            public byte Intensity;
            public HeatPoint(int iX, int iY, byte bIntensity)
            {
                X = iX;
                Y = iY;
                Intensity = bIntensity;
            }
        }

        private List<HeatPoint> HeatPoints = new List<HeatPoint>();
        private int[,] influence;
        private int tileSize = 3;

        public InfluenceMapGUI(int[,] pInfluence, ProxyBot bot)
        {
            InitializeComponent();

            influence = pInfluence;

            int width = influence.GetLength(0);
            int height = influence.GetLength(1);

            this.Width = width * tileSize;
            this.Height = height * tileSize;

            //Draw heat map
            // Create new memory bitmap the same size as the picture box
            Bitmap bMap = new Bitmap(this.Width, this.Height);

            for (var mx = 0; mx < bot.Map.MapWidth; mx++)
            {
                for (var my = 0; my < bot.Map.MapHeight; my++)
                {
                    if (bot.Map.isWalkable(mx, my))
                        HeatPoints.Add(new HeatPoint(mx * tileSize, my * tileSize, 5));
                }
            }

            foreach (Unit u in bot.Units)
            {
                if (u.PlayerID != bot.PlayerID && u.Type.Center)
                {
                    //Enemy center
                    HeatPoints.Add(new HeatPoint(u.X * tileSize, u.Y * tileSize, 100));
                }
            }

            // Call CreateIntensityMask, give it the memory bitmap, and store the result back in the memory bitmap
            bMap = CreateIntensityMask(bMap, HeatPoints);

            int[,] influenceMap = new int[this.Width, this.Height];
            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < this.Height; y++)
                {
                    influenceMap[x, y] = (int)Math.Floor(bMap.GetPixel(x, y).GetBrightness() * this.Width);
                }
            }

            // Colorize the memory bitmap and assign it as the picture boxes image
            this.BackgroundImage = Colorize(bMap, 255);
        }      
        
        private Bitmap CreateIntensityMask(Bitmap bSurface, List<HeatPoint> aHeatPoints)
        {
            // Create new graphics surface from memory bitmap
            Graphics DrawSurface = Graphics.FromImage(bSurface);

            // Set background color to white so that pixels can be correctly colorized
            DrawSurface.Clear(Color.White);

            // Traverse heat point data and draw masks for each heat point
            foreach (HeatPoint DataPoint in aHeatPoints)
            {
                // Render current heat point on draw surface
                //Calculate radius based on intensity, capping at width
                DrawHeatPoint(DrawSurface, DataPoint, (int)DataPoint.Intensity);
            }

            return bSurface;
        }

        private void DrawHeatPoint(Graphics Canvas, HeatPoint HeatPoint, int Radius)
        {
            // Create points generic list of points to hold circumference points
            List<Point> CircumferencePointsList = new List<Point>();

            // Create an empty point to predefine the point struct used in the circumference loop
            Point CircumferencePoint;

            // Create an empty array that will be populated with points from the generic list
            Point[] CircumferencePointsArray;

            // Calculate ratio to scale byte intensity range from 0-255 to 0-1
            float fRatio = 1F / Byte.MaxValue;
            // Precalulate half of byte max value
            byte bHalf = Byte.MaxValue / 2;
            // Flip intensity on it's center value from low-high to high-low
            int iIntensity = (byte)(HeatPoint.Intensity - ((HeatPoint.Intensity - bHalf) * 2));
            // Store scaled and flipped intensity value for use with gradient center location
            float fIntensity = iIntensity * fRatio;

            // Loop through all angles of a circle
            // Define loop variable as a double to prevent casting in each iteration
            // Iterate through loop on 10 degree deltas, this can change to improve performance
            for (double i = 0; i <= 360; i += 10)
            {
                // Replace last iteration point with new empty point struct
                CircumferencePoint = new Point();

                // Plot new point on the circumference of a circle of the defined radius
                // Using the point coordinates, radius, and angle
                // Calculate the position of this iterations point on the circle
                CircumferencePoint.X = Convert.ToInt32(HeatPoint.X + Radius * Math.Cos(ConvertDegreesToRadians(i)));
                CircumferencePoint.Y = Convert.ToInt32(HeatPoint.Y + Radius * Math.Sin(ConvertDegreesToRadians(i)));

                // Add newly plotted circumference point to generic point list
                CircumferencePointsList.Add(CircumferencePoint);
            }

            // Populate empty points system array from generic points array list
            // Do this to satisfy the datatype of the PathGradientBrush and FillPolygon methods
            CircumferencePointsArray = CircumferencePointsList.ToArray();

            // Create new PathGradientBrush to create a radial gradient using the circumference points
            PathGradientBrush GradientShaper = new PathGradientBrush(CircumferencePointsArray);

            // Create new color blend to tell the PathGradientBrush what colors to use and where to put them
            ColorBlend GradientSpecifications = new ColorBlend(3);

            // Define positions of gradient colors, use intesity to adjust the middle color to
            // show more mask or less mask
            GradientSpecifications.Positions = new float[3] { 0, fIntensity, 1 };
            // Define gradient colors and their alpha values, adjust alpha of gradient colors to match intensity
            GradientSpecifications.Colors = new Color[3]
            {
                Color.FromArgb(0, Color.White),
                Color.FromArgb(HeatPoint.Intensity, Color.Black),
                Color.FromArgb(HeatPoint.Intensity, Color.Black)
            };

            // Pass off color blend to PathGradientBrush to instruct it how to generate the gradient
            GradientShaper.InterpolationColors = GradientSpecifications;

            // Draw polygon (circle) using our point array and gradient brush
            Canvas.FillPolygon(GradientShaper, CircumferencePointsArray);
        }

        private double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        public static Bitmap Colorize(Bitmap Mask, byte Alpha)
        {
            // Create new bitmap to act as a work surface for the colorization process
            Bitmap Output = new Bitmap(Mask.Width, Mask.Height, PixelFormat.Format32bppArgb);

            // Create a graphics object from our memory bitmap so we can draw on it and clear it's drawing surface
            Graphics Surface = Graphics.FromImage(Output);
            Surface.Clear(Color.Transparent);

            // Build an array of color mappings to remap our greyscale mask to full color
            // Accept an alpha byte to specify the transparancy of the output image
            ColorMap[] Colors = CreatePaletteIndex(Alpha);

            // Create new image attributes class to handle the color remappings
            // Inject our color map array to instruct the image attributes class how to do the colorization
            ImageAttributes Remapper = new ImageAttributes();
            Remapper.SetRemapTable(Colors);

            // Draw our mask onto our memory bitmap work surface using the new color mapping scheme
            Surface.DrawImage(Mask, new Rectangle(0, 0, Mask.Width, Mask.Height), 0, 0, Mask.Width, Mask.Height, GraphicsUnit.Pixel, Remapper);

            // Send back newly colorized memory bitmap
            return Output;
        }

        private static ColorMap[] CreatePaletteIndex(byte Alpha)
        {
            ColorMap[] OutputMap = new ColorMap[256];

            // Change this path to wherever you saved the palette image.
            Bitmap Palette = (Bitmap)Bitmap.FromFile(@"C:\Projects\Personal\starcraft\StarcraftAI\CSharp\StarcraftAI\pallet.bmp");

            // Loop through each pixel and create a new color mapping
            for (int X = 0; X <= 255; X++)
            {
                OutputMap[X] = new ColorMap();
                OutputMap[X].OldColor = Color.FromArgb(X, X, X);
                OutputMap[X].NewColor = Color.FromArgb(Alpha, Palette.GetPixel(X, 0));
            }

            return OutputMap;
        }
    }
}