﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_lib
{
    class Game_Bomberman_Player
    {
        public int session_id;
        public int x;
        public int y;
        public int hp;

        public Game_Bomberman_Player(int session_id, int x, int y)
        {
            this.session_id = session_id;
            this.x = x;
            this.y = y;
            this.hp = 2;
        }
        public Game_Bomberman_Player(int session_id, int x, int y, int hp)
        {
            this.session_id = session_id;
            this.x = x;
            this.y = y;
            this.hp = hp;
        }

        public void setPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool intersects(int x, int y)
        {
            if (this.x == x && this.y == y) return true;
            return false;
        }
        public bool intersects(List<Tuple<int,int>> coords)
        {
            foreach(Tuple<int, int> x in coords)
            {
                if (this.intersects(x.Item1, x.Item2)) return true;
            }
            return false;
        }
    }
}
