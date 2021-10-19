using WebSocketSharp;
using WebSocketSharp.Server;

namespace SolutionFX
{
    public class PingPongBehavior : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            if (e.Data == "__ping__")
            {
                Send("__pong__");
            }
        }
        
    }
}