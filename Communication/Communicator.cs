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
        private Session session;
        private byte[] buffer;
        private Communication_Package package;
        public Session Session { get => session; set => session = value; }
        public byte[] Buffer { get => buffer; set => buffer = value; }

        public Communicator(Session s)
        {
            this.session = s;
            this.buffer = new byte[1024];
            package = new Communication_Package();
        }

        #region packages
        private Communication_Package ReceivePackage()
        {
            session.Stream.Read(Buffer, 0, Buffer.Length);
            Communication_Package package= new Communication_Package(Buffer);
            package.Interpet();
            return package;
        }

        private void SendPackage(Communication_Package package)
        {
            package.refreshByteArray();
            session.Stream.Write(package.data, 0, package.data.Length);
        }

        #endregion

        #region 
        public int logInOrSignUp()
        {
            //send choiceRequest
            package.SetTypeCHOICE_REQUEST("(0-login/1-signup");
            SendPackage(package);

            //receive anwser
            package = ReceivePackage();
            if (package.packageType == "CHOOSE")
            {
                return Int32.Parse(package.arguments[0]);
            }
            return -1;
        }


        /// <summary>
        /// Allows user to log into an existing account.
        /// </summary>
        /// <param  Client structure="client"></param>
        public void LogIn()
        {
            Authentication auth = new Authentication();
            String username = "";
            String password = "";


            //send request for login information
            package.SetTypeLOGIN_REQUEST();
            SendPackage(package);

            //receive anwser
            package = ReceivePackage();
            if (package.packageType == "LOGIN")
            {
                try
                {
                    username = package.arguments[0];
                    password = package.arguments[1];
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
                        LogIn();
                    }
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
            String password = "";
            String confpassword = "";
            String username = "";

            //send request for signup information
            package.SetTypeSIGNUP_REQUEST();
            SendPackage(package);

            //receive anwser
            package = ReceivePackage();
            if (package.packageType == "SIGNUP")
            {
                username = package.arguments[0];
                password = package.arguments[1];
                confpassword = package.arguments[2];

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
                        SignUp();
                    }
                }
            }
        }
        #endregion

        #region game
        public void chooseGameAndLobby()
        {

            //add test lobby
            GameManager.CreateGame(game_lib.Game.GameName.BOMBERMAN, "TEST LOBBY FOR USER id="+session.Id);

            //send the player currentGameTypes
            SendCurrentGameTypes();
            while (package.packageType != "QUIT_SERVER")
            {
                //let client choose game
                package.SetTypeCHOICE_REQUEST("game choice");
                SendPackage(package);
                package = ReceivePackage();
                if (package.packageType == "CHOICE")
                {
                    int chosenGame = Int32.Parse(package.arguments[0]);
                    while (package.packageType != "BACK")
                    {
                        SendCurrentLobbies(chosenGame);
                        package.SetTypeCHOICE_REQUEST("lobby choice");

                        SendPackage(package);
                        package = ReceivePackage();
                        if(package.packageType== "CHOICE")
                        {
                            GameManager.JoinGame(package.arguments[0], session);
                            while (package.packageType != "QUIT_LOBBY" || package.packageType != "QUIT_GAME")
                            {
                                PlayGame();  
                            }
                        }
                       
                    }
                }
            }
           
        }

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
                    data.Add(g.ToString());
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

