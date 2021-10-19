using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DeveloperFX.Server.Behaviors
{
    public class DefaultBehavior : WebSocketBehavior
    {
        private MainWindow mainWindow;
        private int i;

        public DefaultBehavior(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            i = 0;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            
            
            if (e.Data == "__ping__")
            {
                Send("__pong__");
            }
            else if(e.Data.Length > 0)
            {
                mainWindow.backend.blockingQueue.Add(e.Data);
                i++;
            }
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            if (this.mainWindow.isLaunched)
            {
                Send("__launch__");
                Send("__opened__");
            }
        }
    }
}