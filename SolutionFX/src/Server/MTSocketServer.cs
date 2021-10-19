using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeveloperFX.Server
{
    public class MTSocketServer
    {
        private MainWindow mainWindow;
        private ushort port;
        private ushort tradingPort;
        private TcpListener server;
        private TcpListener tradingServer;

        private IPAddress localAddress;
        private TcpClient client;
        private TcpClient tradingClient;

        private StreamReader streamReader;
        public StreamWriter streamWriter { get; set; }
        private NetworkStream clientStream;

        private StreamReader tradingStreamReader;
        private StreamWriter tradingStreamWriter;
        private NetworkStream tradingClientStream;

        private MTServerStates currentState = MTServerStates.Send;
        private MTServerStates tradingCurrentState = MTServerStates.Send;
        private Backend backend;

        private bool running;

        public MTSocketServer(MainWindow mainWindow, ushort port, ushort tradingPort, Backend backend)
        {
            this.mainWindow = mainWindow;
            this.port = port;
            this.tradingPort = tradingPort;
            this.backend = backend;

            this.localAddress = IPAddress.Parse("127.0.0.1");
        }

        public void run()
        {
            server = new TcpListener(localAddress, port);
            server.Start();

            client = server.AcceptTcpClient();
            mainWindow.infoPage.connected();
            mainWindow.backend.mtOpened();


            clientStream = client.GetStream();
            streamReader = new StreamReader(clientStream);
            streamWriter = new StreamWriter(clientStream);

            running = true;
            while (running)
            {
                switch (currentState)
                {
                    case MTServerStates.Receive:
                        receiveStateAction();
                        break;
                    case MTServerStates.Send:
                        sendStateAction();
                        break;
                }
            }
        }

        public void runTradingServer()
        {
            tradingServer = new TcpListener(localAddress, tradingPort);
            tradingServer.Start();

            tradingClient = tradingServer.AcceptTcpClient();

            tradingClientStream = tradingClient.GetStream();
            tradingStreamReader = new StreamReader(tradingClientStream);
            tradingStreamWriter = new StreamWriter(tradingClientStream);

            running = true;
            while (running)
            {
                switch (tradingCurrentState)
                {
                    case MTServerStates.Receive:
                        receiveTradeStateAction();
                        break;
                    case MTServerStates.Send:
                        sendTradeStateAction();
                        break;
                }
            }
        }

        private void sendTradeStateAction()
        {
            string msg;
            if (backend.tradingBlockingQueue.TryTake(out msg))
            {
                sendMessage(msg, tradingStreamWriter);
                tradingCurrentState = MTServerStates.Receive;
            }
        }

        private void sendStateAction()
        {
            string msg;
            if (backend.blockingQueue.TryTake(out msg))
            {   sendMessage(msg, streamWriter);
                currentState = MTServerStates.Receive;
            }
        }

        private void receiveStateAction()
        {
            string msg = readLineSingleBreak(streamReader);
            if (msg.Length > 0)
            {
                JObject json = JObject.Parse(msg);
                backend.webSocketServer.sendChartResponse(json["Chart"]?.ToString());
                backend.webSocketServer.sendMarketWatchResponse(json["MarketWatch"]?.ToString());
                backend.webSocketServer.sendAccountResponse(json["Account"]?.ToString());
                currentState = MTServerStates.Send;
            }
            
        }

        private void receiveTradeStateAction()
        {
            string msg = readLineSingleBreak(tradingStreamReader);
            if (msg.Length > 0)
            {
                backend.webSocketServer.sendTradeResponse(msg);
                tradingCurrentState = MTServerStates.Send;
            }

        }

        public string readLineSingleBreak(StreamReader reader)
        {
            StringBuilder currentLine = new StringBuilder();
            int i;
            char c;
            bool condition = true;
            while (condition)
            {
                i = reader.Read();
                c = (char)i;
                if (c == '\r' || c == '\n')
                {
                    condition = false;
                }

                currentLine.Append(c);
            }
            //mainWindow.infoPage.setMessageText(currentLine.ToString().Substring(0, 20));
            return currentLine.ToString();
        }

        public void sendMessage(string message, StreamWriter writer)
        {
            writer.WriteLine(message);
            writer.Flush();
        }

        public void stopServer()
        {
            server?.Stop();
        }
    }
}