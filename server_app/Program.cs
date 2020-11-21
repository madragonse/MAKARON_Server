using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using server_lib;

namespace server_app
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress adresIP = IPAddress.Parse("127.0.0.1");
            int port = 9000;
            Server server = new Server(adresIP, port);

            server.Start();
        }
    }
}
