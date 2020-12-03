using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using packages;

namespace game_lib
{

    public class Game_Bomberman : Game
    {

        #region fields
        //TO DO TODO chyba jednak nie ushort
        //ushort[,] map;
        int bombId = 0;
        /// <summary>
        /// Mapa graczy {id_gracza} {{x , y}}
        /// </summary>
        Dictionary<int, Tuple<int, int>> playersPositions = new Dictionary<int, Tuple<int,int>>();
        Dictionary<int, DateTime> bombs = new Dictionary<int, DateTime>();
        //SCORE
        #endregion
        public Game_Bomberman(Session creator) : base()
        {
            this.AddPlayer(creator);
            this.State = GAME_STATE.LOBBY;
        }


        public Game_Bomberman() : base()
        {
            //map = new ushort[20, 20];
        }

        public override GAME_TYPE getGameType()
        {
            return GAME_TYPE.BOMBERMAN;
        }

        //TO DO TODO TUTAJ JĘDRZEJ
        //tutaj przekazana kontrola z wątku klienta
        public override void gameLoop()
        {
            String packageType = "";
            
            while (true)
            {
                //session.ReceivePackage();
                foreach (Session session in this.Sessions)
                {
                    int id = session.id;
                    packageType = session.PackageArguments[0];
                    if (packageType == "PLACE_BOMB")
                    {
                        int bomb_x = Int32.Parse(session.PackageArguments[1]);
                        int bomb_y = Int32.Parse(session.PackageArguments[2]);
                        int bomb_ttl = Int32.Parse(session.PackageArguments[3]);

                        DateTime now = DateTime.Now;
                        DateTime explosionTime = now.AddMilliseconds(bomb_ttl);

                        this.bombs.Add(this.bombId, explosionTime);
                        
                        Bomberman_Package package = new Bomberman_Package();
                        package.SetTypeBOMB_POSITION(this.bombId, bomb_x,bomb_y);

                        this.sendToEveryone(package);

                        this.bombId++;
                    }
                    else if (packageType == "PLAYER_POSITION")
                     {
                         int senderId = Int32.Parse(session.PackageArguments[1]);
                         int x = Int32.Parse(session.PackageArguments[2]);
                         int y = Int32.Parse(session.PackageArguments[3]);

                        playersPositions[session.id] = new Tuple<int,int>(x,y);
                     }
                     else if (packageType == "BOMB_POSITION")
                     {
                         int x = Int32.Parse(session.PackageArguments[1]);
                         int y = Int32.Parse(session.PackageArguments[2]);
                         int ttl = Int32.Parse(session.PackageArguments[3]);
                     }
                }
                ////////////Check for bomb explosions
                DateTime now = DateTime.Now;
                foreach(KeyValuePair<int,DateTime> bomb in bombs)
                {
                    if (bomb.Value.CompareTo(now) >=0)
                    {
                        ///BOMB EXPLOSION
                    }
                }
                ////////////Send players positions
                foreach(KeyValuePair<int, Tuple<int, int>> player in playersPositions)
                {
                    Bomberman_Package package = new Bomberman_Package();
                    package.SetTypePLAYER_POSITION(player.Key,player.Value.Item1,player.Value.Item2);
                    this.sendToExcept(player.Key, package);
                }

            }
        }

        public override void StartGame()
        {
            throw new NotImplementedException();
        }

        public override void StopGame()
        {
            throw new NotImplementedException();
        }

        public override void Update(ulong deltaTime)
        {
            if (this.State == GAME_STATE.IN_GAME)
            {
                //
            }
        }
    }
}
