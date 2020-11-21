using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_lib
{
    class Game_Bomberman : Game
    {
        #region fields
        ushort[,] map;
        //SCORE
        #endregion

        public Game_Bomberman(uint playerLimit, ulong waitingTime) : base(playerLimit, waitingTime)
        {
            map = new ushort[20, 20];
        }

        public override void StartGame()
        {
            throw new NotImplementedException();
        }

        public override void StopGame()
        {
            throw new NotImplementedException();
        }

        public override void updateGame(ulong deltaTime)
        {
            throw new NotImplementedException();
        }
    }
}
