using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace game_lib
{
    /// <summary>
    /// Structure for holding information about individual server client sessions.
    /// That is- the TCP Stream of the client and his unique ID.
    /// </summary>
    public class Player
    {
        public Player(int id, NetworkStream stream)
        {
            Id = id;
            Stream = stream;
        }

        public int Id { get; set; }
        public NetworkStream Stream { get; set; }

    }
}
