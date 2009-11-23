using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProxyBotLib;
using System.Net.Sockets;

namespace StarcraftAI
{
    class Program
    {
        [STAThread]
        public static void Main(System.String[] args)
        {
            StarCraftAgent agent = new StarCraftAgent();
            ProxyBot proxyBot = new ProxyBot(agent);
            proxyBot.showGUI = false;

            try
            {
                proxyBot.start();
            }
            catch (SocketException e)
            {
                e.WriteStackTrace(Console.Error);
                System.Environment.Exit(0);
            }
            catch (System.Exception e)
            {
                e.WriteStackTrace(Console.Error);
            }
        }
    }
}
