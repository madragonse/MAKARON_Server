using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using communication;
using game_lib;

namespace communication
{
    public class GameAndLobbyManager
    {
        #region DEFINITIONS
        //ZMIANA LIMITU GRACZY
        public static uint DEFAULT_PLAYER_LIMIT = 1;
        public static ulong DEFAULT_LOBBY_WAIT_TIME = 6000000; //10minutes
        #endregion

        private static Mutex lobbyListMutex = new Mutex();
        private static Mutex gameListMutex = new Mutex();
        public static List<Lobby> lobbys= new List<Lobby>();
        public static List<Game> games = new List<Game>();

        public static void CreateLobby(String roomName, Game.GAME_TYPE game)
        {
            CreateLobby(roomName, DEFAULT_PLAYER_LIMIT, DEFAULT_LOBBY_WAIT_TIME, game);

        }
        public static void CreateLobby(String roomName,uint playerLimit,ulong waitTime, Game.GAME_TYPE game)
        {
            //check if lobby name is unique
            lobbyListMutex.WaitOne();
            foreach (Lobby l in lobbys)
            {
                if (l.Name == roomName) { throw new Exception("Nazwa lobby zajeta!"); }
            }
            lobbyListMutex.ReleaseMutex();
            
            Game tempG;
            switch (game)
            {
                case Game.GAME_TYPE.BOMBERMAN:
                    tempG = new Game_Bomberman();
                    break;
                default:
                    tempG = new Game_Bomberman();
                    break;
            }
            gameListMutex.WaitOne();
            games.Add(tempG);
            uint gameID = (uint)games.Count() - 1;
            gameListMutex.ReleaseMutex();

            Lobby tempL = new Lobby(roomName, playerLimit, waitTime,gameID,game);
            lobbyListMutex.WaitOne();
            lobbys.Add(tempL);
            lobbyListMutex.ReleaseMutex();
        }

        public static void RemoveLobby(int lobbyId)
        {
            if (lobbyId < lobbys.Count)
            {
                lobbyListMutex.WaitOne();
                lobbys.RemoveAt(lobbyId);
                lobbyListMutex.ReleaseMutex();
            }     
        }

        public static int JoinLobby(String lobbyId, Session session)
        {  
            lobbyListMutex.WaitOne();
            for(int i=0;i<lobbys.Count;i++)
            {
                if (lobbys[i].Name == lobbyId)
                {
                    lobbys[i].AddPlayer(session);
                    lobbyListMutex.ReleaseMutex();
                    return i;
                }
            }
           
            lobbyListMutex.ReleaseMutex();
            return -1; 
        }

        public static void ToogleReadyInLobby(int lobbyIndex, Session session)
        {
            lobbyListMutex.WaitOne();
            lobbys[lobbyIndex].ToogleReady(session);
            lobbyListMutex.ReleaseMutex();
        }

        //MATEUSZ Wyrzuca exception, lobbyIndex poza zakresem
        public static void LeaveLobby(int lobbyIndex, Session session)
        {
            lobbyListMutex.WaitOne();
            lobbys[lobbyIndex].RemovePlayer(session);
            lobbyListMutex.ReleaseMutex();
        }

        public static void Update(ulong deltaTime)
        {
            lobbyListMutex.WaitOne();
            List<Session> sessionsTemp = null;
            foreach (game_lib.Lobby l in lobbys)
            {
                sessionsTemp=l.Update(deltaTime);
                //upon lobby ending transfer sessions to game object
                if (sessionsTemp != null)
                {
                    gameListMutex.WaitOne();
                    games[(int)l.GameID].AddPlayers(sessionsTemp);
                    games[(int)l.GameID].State = Game.GAME_STATE.IN_GAME;
                    games[(int)l.GameID].StartGame();
                    gameListMutex.ReleaseMutex();
                }   
            }
            lobbyListMutex.ReleaseMutex();

            gameListMutex.WaitOne();
            foreach (game_lib.Game g in games)
            {
                g.Update(deltaTime);
            }
            gameListMutex.ReleaseMutex();
        }

        public static List<Lobby> GetAllLobbys() { return lobbys; }
        public static Game getGame(int gameID)
        {
            gameListMutex.WaitOne();
            Game temp = games[gameID];
            gameListMutex.ReleaseMutex();
            return temp;
        }
    }
}
