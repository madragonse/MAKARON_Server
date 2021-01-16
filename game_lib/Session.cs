using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using packages;

namespace game_lib
{

    public class Session
    {
        private byte[] buffer;
        public int id;
        public NetworkStream stream;
        private Communication_Package pingPackage;
        private List<String> packageArguments;
        private Queue<Package> enquedPackages;
        private Mutex QueueMutex = new Mutex();

        #region field_definitions
        public int Id { get => id; set=> id=value; }
        public NetworkStream Stream { get => stream; set=> stream=value; }
        public byte[] Buffer { get => buffer; set => buffer = value; }
        public List<String> PackageArguments { get => packageArguments; set => packageArguments = value; }
        public Queue<Package> EnquedPackages { get => enquedPackages; set => enquedPackages = value; }
        #endregion

        String toString()
        {
            String res = "";
            foreach(String s in packageArguments)
            {
                res += s;
                res += " ";
            }
            return res;
        }

        public Session(int id, NetworkStream stream)
        {
            Id = id;
            Stream = stream;
            this.buffer = new byte[1024];
            this.packageArguments = new List<String>();
            this.pingPackage = new Communication_Package();
            this.pingPackage.SetTypePING();
            this.EnquedPackages = new Queue<Package>();
        }


        public Package ReceivePackageAndSaveToQueue()
        {
            Array.Clear(Buffer, 0, Buffer.Length);
            Stream.Read(Buffer, 0, Buffer.Length);

            //check if it isn't an ammalgamation on packages
            String bufferString = Encoding.ASCII.GetString(Buffer, 0, Buffer.Length);

            String endingTag = "</PACKAGE>";
            //check if there are characterts after endingTag
            List<String> multiplePackageBuffer = new List<String>();
            int endTagIndex = bufferString.IndexOf(endingTag);
            while(endTagIndex < bufferString.Length && endTagIndex>=0)
            {
                endTagIndex += endingTag.Length;
                multiplePackageBuffer.Add(bufferString.Substring(0, endTagIndex));
                bufferString= bufferString.Substring(endTagIndex);
                endTagIndex = bufferString.IndexOf(endingTag);
            }

            String t = bufferString.Substring(0, 1);
            if (bufferString.Substring(0, 1) != "\0")
            {
                multiplePackageBuffer.Add(bufferString);
            }


            Package p= new Package();
            foreach (String package in multiplePackageBuffer)
            {
                p = new Package(package);
                if (p.getArguments()[0] == "PING") { continue; } //ignore ping packages
                QueueMutex.WaitOne();
                this.EnquedPackages.Enqueue(p);
                QueueMutex.ReleaseMutex();
                Console.WriteLine("RECEIVED AND ENQUEUED: " + p.XML);   
            }
           
            return p;
        }

        /// <summary>
        /// Checks if there are any packages waiting in queue to be processed, and saves 
        /// last unporcessed package's arguments to packageArguments field if there are. If not, returns false.
        /// </summary>
        /// <returns>True if there was an unporccessed package, false if there wasn't</returns>
        public Boolean GetLastUnprocessedPackageArguments()
        {
            QueueMutex.WaitOne();
            if (EnquedPackages.Any())
            {
                this.packageArguments = this.EnquedPackages.Dequeue().getArguments();
                QueueMutex.ReleaseMutex();
                return true;
            }
            QueueMutex.ReleaseMutex();
            return false;
        }



        public void ReceivePackage()
        {
            Array.Clear(Buffer, 0, Buffer.Length);
            Stream.Read(Buffer, 0, Buffer.Length);
            Package p = new Package(Buffer);
            //get arguments
            this.packageArguments = p.getArguments();
            //debuging
            Console.WriteLine("RECEIVED: "+p.XML);
        }

        public void Send(byte [] data)
        {
            try
            {
                Stream.Write(data, 0, data.Length);
            }
            catch (Exception) { }
        }

        public void Send(Package package)
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
