﻿using System;
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
        List<Game_Bomberman_Player> players = new List<Game_Bomberman_Player>();
        List<Game_Bomberman_Bomb> bombs = new List<Game_Bomberman_Bomb>();
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

                        DateTime now2 = DateTime.Now;
                        DateTime explosionTime = now2.AddMilliseconds(bomb_ttl);

                        

                        Game_Bomberman_Bomb bomb = new Game_Bomberman_Bomb(this.bombId, bomb_x, bomb_y, explosionTime);
                        this.bombs.Add(bomb);


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

                        this.players.Find(player => player.session_id == session.id).x = x;
                        this.players.Find(player => player.session_id == session.id).y = y;
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
                foreach(Game_Bomberman_Bomb bomb in bombs)
                {
                    if (bomb.explosionTime.CompareTo(now) >=0)
                    {
                        ///BOMB EXPLOSION
                        ///

                        ///Intersects with players
                        List<Tuple<int, int>> explosionCoords = new List<Tuple<int, int>>();
                        foreach(Game_Bomberman_Player player in players)
                        {
                            if(player.intersects(explosionCoords))
                            {
                                //SET DAMAGE TO PLAYER
                            }
                        }
                    }
                }
                ////////////Send players positions
                foreach(Game_Bomberman_Player player in players)
                {
                    Bomberman_Package package = new Bomberman_Package();
                    package.SetTypePLAYER_POSITION(player.session_id,player.x,player.y);
                    this.sendToExcept(player.session_id, package);
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
