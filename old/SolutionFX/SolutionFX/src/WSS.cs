namespace SolutionFX
{
    public class WSS
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

        public void addPingPongBehavior()
        {
            
        }

        public void sendCustomMessage(string msg)
        {
            server.WebSocketServices["/AppWebSiteBehavior"].Sessions.Broadcast(msg);
        }

    }
}