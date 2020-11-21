using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace communication
{
    class Communication_Package
    {
        private enum Types : ushort
        {
            PING,
            QUIT_SERVER,
            QUIT_LOBBY,
            QUIT_GAME
        }

        Types type;
        private byte[] data;

        public void SetTypePING()
        {
            this.type = Communication_Package.Types.PING;
        }

        public void SetTypeQUIT_SERVER()
        {
            this.type = Communication_Package.Types.QUIT_SERVER;
        }

        public void SetTypeQUIT_LOBBY()
        {
            this.type = Communication_Package.Types.QUIT_LOBBY;
        }

        public void SetTypeQUIT_GAME()
        {
            this.type = Communication_Package.Types.QUIT_GAME;
        }


        public byte[] ToByteArray()
        {
            List<Byte> data = new List<byte>();
            byte[] vs = BitConverter.GetBytes((ushort)this.type);

            data.AddRange(vs);
            byte[] result = data.ToArray();
            return result;
        }
    }
}
