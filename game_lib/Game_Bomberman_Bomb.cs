using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_lib
{
    class Game_Bomberman_Bomb
    {
        public int id;
        public int x;
        public int y;
        public int range;
        public DateTime explosionTime;

        public Game_Bomberman_Bomb(int id, int x, int y, DateTime explosionTime)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.range = 2;
            this.explosionTime = explosionTime;
        }

        public Game_Bomberman_Bomb(int id, int x, int y, int range, DateTime explosionTime)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.range = range;
            this.explosionTime = explosionTime;
        }

        public List<Tuple<int,int>> getExplosionCoords()
        {
            List<Tuple<int, int>> temp = new List<Tuple<int, int>>();

            for (int i = (int)(this.x - this.range); i <= (this.x + this.range); i++)
            {
                if (i < 1 || i > 13) continue;
                temp.Add(new Tuple<int, int>(i, this.y));
            }
            for (int i = (int)(this.y - this.range); i <= (this.y + this.range); i++)
            {
                if (i < 1 || i > 13) continue;
                if (i == this.y) continue;
                temp.Add(new Tuple<int, int>(this.x, i));
            }
            return temp;
        }
    }
}
