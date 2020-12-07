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
            this.range = 1;
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

            for(int i = this.x - range; i<this.x+range;i++)
            {
                if (i < 0) continue;
                temp.Add(new Tuple<int, int>(i, this.y));
            }
            for (int i = this.y - range; i < this.y + range; i++)
            {
                if (i < 0) continue;
                temp.Add(new Tuple<int, int>(this.x, i));
            }
            return temp;
        }
    }
}
