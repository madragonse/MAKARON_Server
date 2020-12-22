using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using packages;


namespace game_lib
{
    public abstract class Game
    {
        public enum GAME_STATE
        {
            LOBBY,
            IN_GAME
        }


        public enum GAME_TYPE
        {
            BOMBERMAN,
            SOME_OTHER_GAME
        }

        protected List<Session> sessions;
        protected GAME_STATE state;
        #region field_definitions
        public List<Session> Sessions
        {
            get => sessions;
            set { sessions = value; }
        }
        public GAME_STATE State
        {
            get => state;
            set { state = value; }
        }
        #endregion

        public Game()
        {    
            this.Sessions = new List<Session>();
        }

        public void AddPlayers(List<Session> players)
        {
            this.sessions = players;
        }

        public void AddPlayer(Session p)
        {
            this.sessions.Add(p);
        }
        public void RemovePlayer(Session p)
        {
            this.sessions.Remove(p);
        }

        public uint getNumberOfPlayers()
        {
            return (uint) this.Sessions.Count;
        }

        public void sendToEveryone(Package package)
        {
            foreach(Session s in this.sessions)
            {
                s.Send(package);
            }
        }

        public void sendTo(int receiverId, Package package)
        {
            this.Sessions.Find(x => x.id == receiverId).Send(package);
        }

        public void sendToExcept(int exceptId, Package package)
        {
            this.Sessions.Find(x => x.id != exceptId).Send(package);
        }

        public abstract void Update(ulong deltaTime);
        public abstract void StartGame();
        public abstract void StopGame();
        public abstract GAME_TYPE getGameType();
    }
}
