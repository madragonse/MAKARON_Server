using server_lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_lib
{
    class Lobby
    {
        #region fields
        private List<Session> waiting;
        private uint playerLimit;
        /// <summary>
        /// In seconds, the ammount of time before the game automatically starts
        /// </summary>
        private ulong waitingTime;
        private bool isOver;
        #endregion

        #region field_definitions
        public uint PlayerLimit
        {
            get => playerLimit;
            set{ playerLimit = value;}
        }
        public ulong WaitingTime
        {
            get => waitingTime;
            set { waitingTime = value; }
        }

        public bool IsOver
        {
            get => isOver;
            set { isOver = value; }
        }

        public List<Session> Waiting
        {
            get => waiting;
            set { waiting = value; }
        }
        #endregion
        /// <summary>
        ///  Create new lobby with player limit and waiting time.
        /// </summary>
        /// <param max players in lobby="playerLimit"></param>
        /// <param time after which the game autostarts="waitingTime"></param>
        public Lobby(uint playerLimit, ulong waitingTime)
        {
            PlayerLimit = playerLimit;
            WaitingTime = waitingTime;
            this.isOver = false;
        }

        public bool update(ulong deltaTime)
        {
            this.WaitingTime = this.WaitingTime - deltaTime;
            if (this.WaitingTime >= 0.0 || this.Waiting.Count > this.PlayerLimit)
            {
                EndLobby();
                return true;
            }
            return false;
        }

        public void AddPlayer(Session p)
        {
            if (p != null)
            {
                this.waiting.Add(p);
            }
        }

        public void RemovePlayer(Session p)
        {
            if (p != null)
            {
                this.waiting.Remove(p);
            }
        }

        public List<Session> GetAllPlayers()
        {
            return this.Waiting;
        }

        private void EndLobby()
        {
            this.IsOver = true;
            //clean up the waiting clients list
            foreach(Session w in Waiting)
            {
                if (w == null) { Waiting.Remove(w);}
            }

        }


    }
}
