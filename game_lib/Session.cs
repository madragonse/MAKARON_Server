using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using packages;

namespace game_lib
{

    public class Session
    {
        private byte[] buffer;
        public int id;
        public NetworkStream stream;
        private Communication_Package package;
        private Communication_Package pingPackage;
        private List<String> packageArguments;
        


        #region field_definitions
        public int Id { get => id; set=> id=value; }
        public NetworkStream Stream { get => stream; set=> stream=value; }
        public byte[] Buffer { get => buffer; set => buffer = value; }
        public Communication_Package Package { get => package; set => package = value; }
        public List<String> PackageArguments { get => packageArguments; set => packageArguments = value; }
        #endregion

        public Session(int id, NetworkStream stream)
        {
            Id = id;
            Stream = stream;
            this.buffer = new byte[1024];
            this.packageArguments = new List<String>();
            this.package = new Communication_Package();
            this.pingPackage = new Communication_Package();
            this.pingPackage.SetTypePING();
        }

        public void ReceivePackage()
        {
            Array.Clear(Buffer, 0, Buffer.Length);
            Stream.Read(Buffer, 0, Buffer.Length);
            this.package = new Communication_Package(Buffer);
            //get arguments
            this.packageArguments = package.getArguments();
            //write to console for debbugging purposes
            Console.Write("\nRECEIVED: " + package.XML);
        }

        public void Send(byte [] data)
        {
            try
            {
                Stream.Write(data, 0, data.Length);
                Console.Write("\nSENT: " + package.XML);
            }
            catch (Exception) { }
        }

        public void Send(Communication_Package package)
        {
            try
            {
                byte[] data = package.ToByteArray();
                Stream.Write(data, 0, data.Length);
                Console.Write("\nSENT: " + package.XML);
            }
            catch (Exception) { }
        }

        public void Ping()
        {
            Send(pingPackage);
        }

    }
}
