using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionFX
{
    class Backend
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
