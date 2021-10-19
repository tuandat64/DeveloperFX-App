using DeveloperFX.Server.Behaviors;
using WebSocketSharp.Server;

namespace DeveloperFX.Server
{
    public class SiteWebSocketServer
    {
        private MainWindow mainWindow;
        private ushort port;
        private WebSocketServer server;
        private Backend backend;

        public SiteWebSocketServer(MainWindow mainWindow, ushort port, Backend backend)
        {
            this.mainWindow = mainWindow;
            backend = mainWindow.backend;
            this.port = port;
            this.backend = backend;
            server = new WebSocketServer("ws://localhost:" + port);
            server.AddWebSocketService<DefaultBehavior>("/DefaultBehavior", () => new DefaultBehavior(mainWindow));
            server.AddWebSocketService<ChartBehavior>("/ChartBehavior");
            server.AddWebSocketService<MarketWatchBehavior>("/MarketWatchBehavior");
            server.AddWebSocketService<AccountBehavior>("/AccountBehavior");
            server.AddWebSocketService<TradeBehavior>("/TradeBehavior", () => new TradeBehavior(mainWindow));
            server.Start();
        }

        public void stopServer()
        {
            server.Stop();
        }

        public void sendTradeResponse(string msg)
        {
            server.WebSocketServices["/TradeBehavior"].Sessions.Broadcast(msg);
        }

        public void sendCustomMessage(string msg)
        {
            server.WebSocketServices["/DefaultBehavior"].Sessions.Broadcast(msg);
        }

        public void sendChartResponse(string msg)
        {
            server.WebSocketServices["/ChartBehavior"].Sessions.Broadcast(msg);
        }

        public void sendMarketWatchResponse(string msg)
        {
            server.WebSocketServices["/MarketWatchBehavior"].Sessions.Broadcast(msg);
        }

        public void sendAccountResponse(string msg)
        {
            server.WebSocketServices["/AccountBehavior"].Sessions.Broadcast(msg);
        }

    }
}