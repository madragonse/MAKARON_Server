using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using game_lib;


namespace communication
{
    public class Communicator
    {

        public enum COMMUNICATION_STATE
        {
           LOGIN_SIGNUP,
           GAME_LOBBY,
           PLAYING
        }
        private Player session;
        private byte[] buffer;
        private Communication_Package package;
        private Communication_Package pingPackage;
        private List<String> packageArguments;

        private COMMUNICATION_STATE state;

        public Player Session { get => session; set => session = value; }
        public byte[] Buffer { get => buffer; set => buffer = value; }

        public Communicator(Player s)
        {
            this.session = s;
            this.buffer = new byte[1024];
            package = new Communication_Package();
            pingPackage= new Communication_Package();
            pingPackage.SetTypePING();
        }

        #region packages
        private Communication_Package ReceivePackage()
        {
            Array.Clear(Buffer, 0, Buffer.Length);
            Session.Stream.Read(Buffer, 0, Buffer.Length);
            Communication_Package package = new Communication_Package(Buffer);
            //get arguments
            this.packageArguments = package.getArguments();
            //write to console for debbugging purposes
            Console.Write("\nRECEIVED: "+package.XML);
            return package;
        }

        private void SendPackage(Communication_Package package)
        {
            if (this.Session.Stream != null)
            {
                byte[] data = package.ToByteArray();
                Session.Stream.Write(data, 0, data.Length);
                Console.Write("\nSENT: " + package.XML);
            }
        }
        public void Ping()
        {
            SendPackage(pingPackage);
        }
        #endregion


        public void BeginCommunication()
        {
            //set session timeout for 5seconds
            this.Session.Stream.ReadTimeout = 5000;

            String packageType = "";
            this.state = COMMUNICATION_STATE.LOGIN_SIGNUP;

            try
            {
                while (true)
                {
                    package = ReceivePackage();
                    packageType = packageArguments[0];

                    if (packageType == "PING") { continue; } //ignore ping packages

                    if (this.state == COMMUNICATION_STATE.LOGIN_SIGNUP)
                    {
                        if (packageType == "SIGNUP") { SignUp(); }
                        if (packageType == "LOGIN") { LogIn(); }
                        if (packageType == "REQUEST_GAME_LIST")
                        {
                            SendCurrentGameTypes();
                            this.state = COMMUNICATION_STATE.GAME_LOBBY;
                        }
                    }
                    if (this.state == COMMUNICATION_STATE.GAME_LOBBY)
                    {
                        if (packageType == "REQUEST_LOBBY_LIST")
                        {
                            SendCurrentLobbies(Int32.Parse(packageArguments[1]));
                        }
                        if (packageType == "CREATE_LOBBY")
                        {
                            game_lib.Game.GameName game = (game_lib.Game.GameName)Enum.Parse(typeof(game_lib.Game.GameName), packageArguments[2]);
                            GameManager.CreateGame(game, packageArguments[1]);
                        }
                        if (packageType == "JOIN_LOBBY")
                        {
                            GameManager.JoinGame(Int32.Parse(packageArguments[1]), session);
                            this.PlayGame();
                        }
                    }

                }
            }
            catch (Exception)
            {
                Console.Write("\n\rPlayer " + Session.Id + " has dissconected :(");
                this.Session = null;
            }
           
        }

        #region authentication
        /// <summary>
        /// Allows user to log into an existing account.
        /// </summary>
        /// <param  Client structure="client"></param>
        public void LogIn()
        {
            Authentication auth = new Authentication();
            String username = "";
            String password = "";

            try
            {
                username = packageArguments[1];
                password = packageArguments[2];
                auth.AuthorizeUser(username, password);
                Console.WriteLine("\nA user loged in [username:" + username + "]");

                package.SetTypeLOGIN_CONFIRM(username);
                SendPackage(package);
            }
            catch (AuthenticationException e)
            {
                if (e.ErrorCategory == -1)
                {
                    package.SetTypeERROR("Server malfunction: " + e);
                    SendPackage(package);
                    return;
                }
                if (e.ErrorCategory == 1)
                {
                    package.SetTypeLOGIN_REFUSE(username, e.ToString());
                    SendPackage(package);
                }
            }
        }

        /// <summary>
        /// Allows user to create a new account.
        /// </summary>
        /// <param  Client structure="client"></param>
        public void SignUp()
        {
            Authentication auth = new Authentication();
            String username = packageArguments[1];
            String password = packageArguments[2];

            try
            {
                auth.CreateUser(username, password);
                Console.WriteLine("\nNew account created [user:" + username + " password: " + password + "]");
                package.SetTypeSIGNUP_CONFIRM(username);
                SendPackage(package);
            }
            catch (AuthenticationException e)
            {
                if (e.ErrorCategory == -1)
                {
                    package.SetTypeERROR("Server malfunction: " + e);
                    SendPackage(package);
                    return;
                }
                if (e.ErrorCategory == 1)
                {
                    package.SetTypeSIGNUP_REFUSE(username, e.ToString());
                    SendPackage(package);
                }
            }
           
        }

   
        #endregion

        #region game


        private void PlayGame()
        {
            //doGameStuffHere
        }


        private void SendCurrentLobbies(int gameId)
        {
            List<String> data = new List<string>();
            List<Game> currGames = GameManager.GetAllGames();
            foreach (Game g in currGames)
            {
                if ((int)g.getGameType() == gameId)
                {
                    foreach(String gameData in g.getData())
                    {
                        data.Add(gameData);
                    } 
                }
            }
            package.SetTypeLIST(data);
            SendPackage(package);
        }

        private void SendCurrentGameTypes()
        {
            List<String> data = new List<string>();
            var values = Enum.GetValues(typeof(game_lib.Game.GameName));
            foreach (var v in values)
            {
                data.Add(v.ToString());
            }
            package.SetTypeLIST(data);
            SendPackage(package);
        }
        #endregion
    }
}

