using System;
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
using xRay.Toolkit.Utilities;

namespace Debugger.Influence
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
        private float[,] influence;
        private int tileSize = 3;

        public InfluenceMapGUI(float[,] pInfluence, ProxyBot bot)
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

            // Call CreateIntensityMask, give it the memory bitmap, and store the result back in the memory bitmap
            bMap = CreateIntensityMask(bMap);

            int[,] influenceMap = new int[this.Width, this.Height];
            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < this.Height; y++)
                {
                    influenceMap[x, y] = (int)Math.Floor(bMap.GetPixel(x, y).GetBrightness() * this.Width);
                }
            }

            // Colorize the memory bitmap and assign it as the picture boxes image
            this.BackgroundImage = bMap;//Colorize(bMap, 255);
        }      
        
        private Bitmap CreateIntensityMask(Bitmap bSurface)
        {
            // Create new graphics surface from memory bitmap
            Graphics DrawSurface = Graphics.FromImage(bSurface);

            // Set background color to white so that pixels can be correctly colorized
            DrawSurface.Clear(Color.White);

            //Normalize influence values over a range from 0 to 1
            float highest = 0f;
            for (var x = 0; x < influence.GetLength(0); x++)
            {
                for (var y = 0; y < influence.GetLength(1); y++)
                {
                    if (influence[x, y] > highest) highest = influence[x, y];
                }
            }

            for (var x = 0; x < influence.GetLength(0); x++)
            {
                for (var y = 0; y < influence.GetLength(1); y++)
                {
                    var ratio = influence[x, y] / highest;
                    Color alpha = Color.Black;
                    alpha = RGBHSL.SetBrightness(alpha, ratio);
                    SolidBrush valueBrush = new SolidBrush(alpha);
                    DrawSurface.FillRectangle(valueBrush, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
                }
            }

            return bSurface;
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
            Bitmap Palette = (Bitmap)Bitmap.FromFile(@"C:\Projects\Personal\starcraft\StarcraftAI\CSharp\Debugger\pallet.bmp");

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