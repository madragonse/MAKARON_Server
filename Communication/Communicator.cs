using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using game_lib;
using database;
using packages;

namespace communication
{
    public class Communicator
    {

        public enum COMMUNICATION_STATE
        {
           LOGIN_SIGNUP,
           SERVER,
           LOBBY,
           GAME
        }

        #region fields
        private Session session;
        private Communication_Package cpackage;
        private COMMUNICATION_STATE state;
        private int currentLobbyId;
        private int currentGameId;
        #endregion

        #region field_definitions
        public Session Session { get => session; set => session = value; }
        public COMMUNICATION_STATE State { get=>state; set=>state=value; }
        #endregion

        public Communicator(Session s)
        {
            this.session = s;
            this.currentGameId = -1;
            this.currentLobbyId = -1;
            this.cpackage = new Communication_Package();
        }


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
                    session.ReceivePackage();
                    packageType = session.PackageArguments[0];

                    if (packageType == "PING") { continue; } //ignore ping packages

                    if (this.state == COMMUNICATION_STATE.LOGIN_SIGNUP)
                    {
                        if (packageType == "SIGNUP") { SignUp(); }
                        if (packageType == "LOGIN") { LogIn(); }
                        if (packageType == "REQUEST_LOBBY_LIST")
                        {
                            //DEBUG
                            GameAndLobbyManager.CreateLobby("LOBBY FOR PLAYER "+session.Id.ToString(), Game.GAME_TYPE.BOMBERMAN);
                            SendCurrentLobbies();
                            this.state = COMMUNICATION_STATE.SERVER;
                        }
                    }
                    if (this.state == COMMUNICATION_STATE.SERVER)
                    {
                        if (packageType == "CREATE_LOBBY") { CreateLobby(); }
                        if (packageType == "JOIN_LOBBY") { JoinLobby();  }
                        if (packageType == "REQUEST_LOBBY_LIST") { SendCurrentLobbies();}
                        if (packageType == "REQUEST_LOBBY_LIST_ARG") { SendCurrentLobbiesForChosenGame(); }
                    }
                    if (this.state == COMMUNICATION_STATE.LOBBY)
                    {
                        if (this.currentLobbyId == -1){ this.state = COMMUNICATION_STATE.SERVER;}
                        LobbyLoop(); 
                    }
                    if (this.state == COMMUNICATION_STATE.GAME)
                    {
                        if (this.currentGameId == -1) { this.state = COMMUNICATION_STATE.SERVER; }
                        GameLoop();
                        this.state = COMMUNICATION_STATE.LOBBY;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("\n\rPlayer " + Session.Id + " has dissconected :(");
                this.Session = null;
                //quit lobby or game
                if (currentLobbyId > -1)  { GameAndLobbyManager.LeaveLobby(this.currentLobbyId, this.session); }
                //TODO quit game
               
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
                username = session.PackageArguments[1];
                password = session.PackageArguments[2];
                auth.AuthorizeUser(username, password);
                Console.WriteLine("\nA user loged in [username:" + username + "]");

                cpackage.SetTypeLOGIN_CONFIRM(username);
                session.Send(cpackage);
            }
            catch (AuthenticationException e)
            {
                if (e.ErrorCategory == -1)
                {
                    cpackage.SetTypeERROR("Server malfunction: " + e);
                    session.Send(cpackage);
                    return;
                }
                if (e.ErrorCategory == 1)
                {
                    cpackage.SetTypeLOGIN_REFUSE(username, e.ToString());
                    session.Send(cpackage);
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
            String username = session.PackageArguments[1];
            String password = session.PackageArguments[2];

            try
            {
                auth.CreateUser(username, password);
                Console.WriteLine("\nNew account created [user:" + username + " password: " + password + "]");
                cpackage.SetTypeSIGNUP_CONFIRM(username);
                session.Send(cpackage);
            }
            catch (AuthenticationException e)
            {
                if (e.ErrorCategory == -1)
                {
                    cpackage.SetTypeERROR("Server malfunction: " + e);
                    session.Send(cpackage);
                    return;
                }
                if (e.ErrorCategory == 1)
                {
                    cpackage.SetTypeSIGNUP_REFUSE(username, e.ToString());
                    session.Send(cpackage);
                }
            }
           
        }

   
        #endregion

        #region game
        private void JoinLobby()
        {
            try
            {
                this.currentLobbyId = GameAndLobbyManager.JoinLobby(session.PackageArguments[1], session);
                cpackage.SetTypeJOIN_LOBBY_CONFIRM();
                session.Send(cpackage);
                this.state = COMMUNICATION_STATE.LOBBY;
            }
            catch(Exception e)
            {
                cpackage.SetTypeJOIN_LOBBY_REFUSE(e.ToString());
                session.Send(cpackage);
                return;
            }
            
        }

        private void CreateLobby()
        {
            try
            {
                game_lib.Game.GAME_TYPE game = (game_lib.Game.GAME_TYPE)Enum.Parse(typeof(game_lib.Game.GAME_TYPE), session.PackageArguments[2]);
                GameAndLobbyManager.CreateLobby(session.PackageArguments[1], game);
            }
            catch (Exception e)
            {
                cpackage.SetTypeCREATE_LOBBY_REFUSE(e.ToString());
                session.Send(cpackage);
                return;
            }

        }
        private void GameLoop()
        {
            Package p = new Package();
            while (true)
            {
              p = session.ReceivePackageAndSaveToQueue();
              //check if game hasn't "kicked the player out"
            }
        }

        private void LobbyLoop()
        {
            Package p = new Package();
            while (!GameAndLobbyManager.lobbys[currentLobbyId].IsOver )
            {
                session.ReceivePackageAndSaveToQueue();
                //gets last unpocessed package arguments into the sessions packageArguments field
                while (session.GetLastUnprocessedPackageArguments())
                {
                    String packageType = session.PackageArguments[0];

                    if (packageType == "LOBBY_READY")
                    {
                        GameAndLobbyManager.ToogleReadyInLobby(this.currentLobbyId,session);
                        Console.WriteLine("TOOGLED PLAYER READY");
                    }
                    if (packageType == "QUIT_LOBBY")
                    {
                        GameAndLobbyManager.LeaveLobby(this.currentLobbyId, session);
                        this.state = COMMUNICATION_STATE.SERVER;
                    }
                }
            }
            this.state = COMMUNICATION_STATE.GAME;
        }


        private void SendCurrentLobbies()
        {
            List<String> data = new List<string>();
            List<Lobby> currLobbys = GameAndLobbyManager.GetAllLobbys();
            foreach (Lobby l in currLobbys)
            {
                data.Add(l.toString());
            }
            cpackage.SetTypeLIST(data);
            session.Send(cpackage);
        }

        private void SendCurrentLobbiesForChosenGame()
        {
            List<String> data = new List<string>();
            List<Lobby> currLobbys = GameAndLobbyManager.GetAllLobbys();
            Game.GAME_TYPE game= (game_lib.Game.GAME_TYPE)Enum.Parse(typeof(game_lib.Game.GAME_TYPE), session.PackageArguments[1]);
            foreach (Lobby l in currLobbys)
            {
                if (l.getGameType() == game){ data.Add(l.toString());  }
            }
            cpackage.SetTypeLIST(data);
            session.Send(cpackage);
        }
        #endregion
    }
}

