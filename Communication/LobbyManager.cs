using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using communication;

namespace communication
{
    public class LobbyManager
    {

        public static List<game_lib.Lobby> lobbys= new List<game_lib.Lobby>();

        public static void CreateLobby(game_lib.Game.GameName name,String roomName)
        {
            switch (name)
               {
                   case game_lib.Game.GameName.BOMBERMAN:
                       lobbys.Add(new game_lib.Lobby(10, 30, roomName, name));
                       break;
                   default: break;
               }
        }
        public static void RemoveGame(int lobbyId)
        {
            lobbys.RemoveAt(lobbyId);
        }

        public static List<game_lib.Lobby> GetAllLobbys()
        {
            return lobbys;
        }

        public static bool JoinLobby(int lobbyId, game_lib.Session session)
        {
            if (lobbyId > lobbys.Count) { return false; }
            return lobbys[lobbyId].AddPlayer(session);
        }

        public static void update(ulong deltaTime)
        {
            foreach(game_lib.Lobby l in lobbys)
            {
                l.update(deltaTime);
            }
        }
        
    }
}
