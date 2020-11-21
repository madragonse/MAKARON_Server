using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using game_lib;

namespace server_lib
{
    class GameManager
    {
        public enum GameName
        {
            BOMBERMAN
        }

        public static List<Game> games= new List<Game>();

        public static void CreateGame(GameName name,String roomName)
        {
            switch (name)
            {
                case GameName.BOMBERMAN:
                    games.Add(new Game_Bomberman(10,30,roomName));
                    break;
                default: break;
            }
        }
        public static void RemoveGame(int gameId)
        {
            games.RemoveAt(gameId);
        }

        public static List<Game> GetAllGames()
        {
            return games;
        }
        
    }
}
