using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DeveloperFX.Server.Behaviors
{
    public class TradeBehavior : WebSocketBehavior
    {
        private MainWindow mainWindow;

        public TradeBehavior(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            if (e.Data.Length > 0)
            {
                mainWindow.backend.tradingBlockingQueue.Add(e.Data);
            }
        }
    }
}