namespace SolutionFX
{
    public class Backend
    {
        private int port = 27401;
        private WSS wss;
        public Backend()
        {
            
            this.wss = new WSS(this.port);
            this.wss.startServer();
        }
    }
}