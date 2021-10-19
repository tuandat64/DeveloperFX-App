namespace SolutionFX
{
    public class Backend
    {
        private int port = 27401;
        private WSS wss;
        
        
        public Backend()
        {
            wss = new WSS(port);
        }

        public WSS getWss()
        {
            return wss;
        }

        public void launchedClicked()
        {
            wss.launchedClicked();
            wss.sendCustomMessage("__launch__");
        }
    }
}