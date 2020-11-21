using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using communication;

namespace server_lib
{
    public class Server
    {

            #region fields
            IPAddress ipAddress;
            int port;
            int buffer_size = 1024;
            bool running;
            List<Communicator> communicators = new List<Communicator>();
            int clientCounter = 0;
            TcpListener tcpListener;
    
            public delegate void TransmissionDataDelegate(game_lib.Session client);
            #endregion

            #region field_definitions
            public IPAddress IPAddress
            {
                get => ipAddress;
                set
                {
                    if (!running) ipAddress = value;
                    else throw new Exception("The server is not curently running");
                }
            }

            public int Port
            {
                get => port;
                set
                {
                    int tmp = port;
                    if (!running) port = value; else throw new Exception("Cannot change the port whilst the server is running");
                    if (!checkPort())
                    {
                        port = tmp;
                        throw new Exception("Illegal port value");
                    }

                }

            }

            public int Buffer_size
            {
                get => buffer_size; set
                {
                    if (value < 0 || value > 1024 * 1024 * 64) throw new Exception("Illegal packet size");
                    if (!running) buffer_size = value; else throw new Exception("Cannot change packet size while the server is running");
                }

            }
            protected TcpListener TcpListener { get => tcpListener; set => tcpListener = value; }
            public List<Communicator> Communciators { get => communicators; set => communicators = value; }
            #endregion

            #region Constructors



            public Server(IPAddress IP, int port)

            {
                running = false;
                IPAddress = IP;
                Port = port;
                if (!checkPort())
                {
                    Port = 8000;
                    throw new Exception("illegal port, port set to 8000");
                }

            }

            #endregion

            #region Functions
            protected bool checkPort()
            {
                if (port < 1024 || port > 49151) return false;
                return true;
            }


            protected void StartListening()
            {
                TcpListener = new TcpListener(IPAddress, Port);
                TcpListener.Start();
            }

            protected void AcceptClient()
            {
                while (true)
                {
                    TcpClient tcpClient = TcpListener.AcceptTcpClient();
                    Console.Write("\nNew client connected! Client id: " + clientCounter);

                    NetworkStream stream = tcpClient.GetStream();
                    game_lib.Session client= new game_lib.Session(clientCounter, stream);
                    //create a new buffer for the client
                    communicators.Add(new Communicator(client));
                    
                    TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                    transmissionDelegate.BeginInvoke(client, TransmissionCallback, tcpClient);
                    clientCounter++;
                }
            }

            private void TransmissionCallback(IAsyncResult ar)
            {
                TcpClient tcpClient = (TcpClient)ar.AsyncState;
                tcpClient.Close();

            }

            /// <summary>
            /// Welcomes given client to the server and allows him to proceed to log in or sign up.
            /// </summary>
            /// <param name="client"></param>
            protected void BeginDataTransmission(game_lib.Session client)
            {

                String bufferString = communicators[client.Id].greetAndChooseOption();

                if (bufferString == "s") { communicators[client.Id].SignUp(client); }
                //after user signed up, make him log in
                communicators[client.Id].LogIn(client);
                //after sucesfull loging in, echo the client
                communicators[client.Id].LetPlay(client);
            }


            /// <summary>
            /// Starts the operations of the server.
            /// </summary>
            public void Start()
            {
                Console.Write("\nStarting up the server...");
                StartListening();
                Console.Write("\nListening for clients commenced...");
                AcceptClient();
            }

            #endregion
    }
    
}
