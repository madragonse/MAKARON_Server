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
        public static uint DEFAULT_PLAYER_LIMIT = 10;
        public static ulong DEFAULT_LOBBY_WAIT_TIME = 6000000; //1minute
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

        public static Lobby JoinLobby(String lobbyId, Session session)
        {  
            lobbyListMutex.WaitOne();
            foreach(Lobby l in lobbys)
            {
                if (l.Name == lobbyId)
                {
                    l.AddPlayer(session);
                    lobbyListMutex.ReleaseMutex();
                    return l;
                }
            }
           
            lobbyListMutex.ReleaseMutex();
            return null; 
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
