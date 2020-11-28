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
        public override void gameLoop(Session session)
        {
            String packageType = "";

            while (true)
            {
                session.ReceivePackage();
                packageType = session.PackageArguments[0];


                if (packageType == "PLACE_BOMB")
                {

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
