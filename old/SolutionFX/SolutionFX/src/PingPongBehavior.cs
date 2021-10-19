namespace SolutionFX
{
    public class PingPongBehavior : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            if (e.Data.ToString() == "__ping__")
            {
                Send("__pong__");
            }
        }

        public void sendCustomMessage(string msg)
        {
            Send(msg);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            base.OnError(e);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
        }
    }
}