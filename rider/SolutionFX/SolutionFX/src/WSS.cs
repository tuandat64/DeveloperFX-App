using WebSocketSharp.Server;

namespace SolutionFX
{
    public class WSS
    {
        private WebSocketServer server;

        private int port;
        public WSS(int port)
        {
            this.port = port;
            server = new WebSocketServer("ws://localhost:" + port);
            addPingPongBehavior();
            server.Start();
            
        }

        public void launchedClicked()
        {
            server.AddWebSocketService<AccountBehavior>("/AccountBehavior");
            server.AddWebSocketService<PairsBehavior>("/PairsBehavior");
            server.AddWebSocketService<TickBehavior>("/TickBehavior");
        }
        
        public void addPingPongBehavior()
        {
            server.AddWebSocketService<PingPongBehavior>("/PingPongBehavior");
        }

        public void sendCustomMessage(string msg)
        {
            server.WebSocketServices["/PingPongBehavior"].Sessions.Broadcast(msg);
        }

    }
}