using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using Debugger.Influence;
using StarcraftAI;
using ProxyBotLib;
using System.Net.Sockets;

namespace Debugger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            StarCraftAgent agent = new StarCraftAgent();
            ProxyBot proxyBot = new ProxyBot(agent);
            proxyBot.showGUI = true;

            //Start the agent in it's own thread
            BackgroundWorker proxyBotBg = new BackgroundWorker();
            proxyBotBg.DoWork += new DoWorkEventHandler((object sender, DoWorkEventArgs e) =>
            {
                proxyBot.start();
            });
            proxyBotBg.RunWorkerAsync();

            //Sign up for the proxy bot connected event
            Toolbar toolbar = new Toolbar(proxyBot, agent);
            Application.Run(toolbar);
        }
    }
}
