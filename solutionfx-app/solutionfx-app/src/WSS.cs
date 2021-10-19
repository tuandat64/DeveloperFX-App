using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SolutionFX
{
    class WSS
    {
        private WebSocketServer server;

        private int port;
        public WSS(int port)
        {
            this.port = port;
            this.server = new WebSocketServer("ws://localhost:" + port);
            
            this.server.Start();
            
        }

       
        public void startServer()
        {

        }

        public void add

        public void sendCustomMessage(string msg)
        {
            server.WebSocketServices["/AppWebSiteBehavior"].Sessions.Broadcast(msg);
        }

    }
}
