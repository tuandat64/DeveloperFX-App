using System.Collections.Concurrent;
using System.Threading;

namespace DeveloperFX.Server
{
    public class Backend
    {
        public MainWindow mainWindow { get; set; }

        private ushort webSocketServerPort = 27401;
        private ushort socketServerPort = 27402;
        private ushort socketServerTradingPort = 27403;

        public MTSocketServer socketServer { get; }
        public SiteWebSocketServer webSocketServer { get; }

        public BlockingCollection<string> blockingQueue { get; }
        public BlockingCollection<string> tradingBlockingQueue { get; }

        public bool isConnectedToMT { get; set; } = false;

        public Backend(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            this.blockingQueue = new BlockingCollection<string>(1);
            this.tradingBlockingQueue = new BlockingCollection<string>(1);
            webSocketServer = new SiteWebSocketServer(mainWindow, webSocketServerPort, this);
            socketServer = new MTSocketServer(mainWindow, socketServerPort, socketServerTradingPort, this);
        }

        public void sendWebSiteCustomMessage(string msg)
        {
            webSocketServer.sendCustomMessage(msg);
        }

        public void launchSocketServer()
        {
            Thread socketServerThread = new Thread(new ThreadStart(socketServer.run));
            socketServerThread.Start();
            Thread socketServerTradingThread = new Thread(new ThreadStart(socketServer.runTradingServer));
            socketServerTradingThread.Start();
            isConnectedToMT = true;
        }

        public void mtOpened()
        {
            webSocketServer.sendCustomMessage("__opened__");
        }

        public void close()
        {
            webSocketServer.stopServer();
            socketServer.stopServer();
        }
    }
}