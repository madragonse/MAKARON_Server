using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using communication;

namespace communication
{
    public class GameManager
    {


        public static List<game_lib.Game> games= new List<game_lib.Game>();

        public static void CreateGame(game_lib.Game.GameName name,String roomName)
        {
            switch (name)
            {
                case game_lib.Game.GameName.BOMBERMAN:
                    games.Add(new game_lib.Game_Bomberman(10,30,roomName));
                    break;
                default: break;
            }
        }
        public static void RemoveGame(int gameId)
        {
            games.RemoveAt(gameId);
        }

        public static List<game_lib.Game> GetAllGames()
        {
            return games;
        }

        public static bool JoinGame(int gameId, game_lib.Session session)
        {
            if (gameId > games.Count) { return false; }
            return games[gameId].AddPlayer(session);
        }

        public static void update(ulong deltaTime)
        {
            foreach(game_lib.Game g in games)
            {
                g.update(deltaTime);
            }
        }
        
    }
}
