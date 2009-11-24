using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProxyBotLib;
using Debugger.Influence;
using StarcraftAI;

namespace Debugger
{
    public partial class Toolbar : Form
    {
        private ToolStripStatusLabel proxyStatus;
        private ProxyBot proxyBot;
        private StarCraftAgent agent;

        public Toolbar(ProxyBot pProxyBot, StarCraftAgent pAgent)
        {
            InitializeComponent();

            proxyBot = pProxyBot;
            agent = pAgent;

            //Subscribe to events
            proxyBot.OnStarcraftConnected += new ProxyBot.ProxyBotDelegate(proxyBot_OnStarcraftConnected);
            
            //Set up status bar
            proxyStatus = new ToolStripStatusLabel("Proxy Bot: Waiting for Starcraft...");
            ssMainStatus.Items.Add(proxyStatus);
        }

        void proxyBot_OnStarcraftConnected(EventArgs e)
        {
            proxyStatus.Text = "Proxy Bot: Starcraft Connected";
        }

        private void btnInfluenceMap_Click(object sender, EventArgs e)
        {
            float[,] currentMap = agent.InfluenceMap.GetMap(InfluenceMap.Terrain.GROUND);
            InfluenceMapGUI imGui = new InfluenceMapGUI(currentMap, proxyBot);
            imGui.Show();
        }
    }
}
