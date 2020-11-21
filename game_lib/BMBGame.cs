using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_lib
{
    class BMBGame : Game
    {
        public BMBGame(uint playerLimit, ulong waitingTime) : base(playerLimit, waitingTime) { }

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
