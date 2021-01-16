using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace game_lib
{
    public class Lobby
    {

        public class LobbyPlayer
        {
            public bool ready;
            
            public Session Session { get; set; }
            public bool Ready { get=>ready;
                                set {ready=value; }
            }

            public void SetReady(bool b)
            {
                this.Ready = b;
            }

            public LobbyPlayer(Session s)
            {
                this.Session = s;
                this.ready = false;
            }
        } 


        #region fields
        private String roomName;
        private List<LobbyPlayer> waitingPlayers;
        private uint playerLimit;
        private ulong waitingTime;
        private uint gameID;
        private Game.GAME_TYPE gameType;
        private bool isOver;
        private static Mutex playerListMutex = new Mutex();
        #endregion

        #region field_definitions
        public uint PlayerLimit
        {
            get => playerLimit;
            set { playerLimit = value; }
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

        public List<LobbyPlayer> WaitingPlayers
        {
            get => waitingPlayers;
            set { waitingPlayers = value; }
        }

        public String Name
        {
            get => roomName;
            set { roomName = value; }
        }

        public uint GameID
        {
            get => gameID;
            set { gameID = value; }
        } 
        #endregion


        public Lobby(String roomName, uint playerLimit, ulong waitingTime, uint gameID, Game.GAME_TYPE gameType)
        {
            this.roomName = roomName;
            PlayerLimit = playerLimit;
            WaitingTime = waitingTime;
            this.WaitingPlayers = new List<LobbyPlayer>();
            isOver = false;
            this.gameID = gameID;
            this.gameType = gameType;
        }



        public List<Session> Update(ulong deltaTime)
        {
            if (!this.isOver)
            {

                this.WaitingTime = this.WaitingTime - deltaTime;
                //CleanUpNotExistingPlayers();

                //if waiting time runs out end lobby regardless of anything else
                //if player limit is reached and all players are ready, end lobby
                if (this.WaitingTime <= 0.0 || (this.getNumberOfPlayers() >= this.PlayerLimit && allPlayersReady()))
                {
                    this.isOver = true;
                    return getAllSessions();
                }
            }
            return null;
        }

 
        public Lobby AddPlayer(Session p)
        {
            if (p == null){ return null; }
            if (CheckIfAlreadyJoined(p)) { throw new GameException("Player arleady joined this lobby"); }
            if (waitingPlayers.Count >= playerLimit) { throw new GameException("This Lobby is full"); }
            if (isOver) { throw new GameException("Lobby currently in game"); }

            this.waitingPlayers.Add(new LobbyPlayer(p));
            return this;
        }


        public void RemovePlayer(Session p)
        {
            if (p == null) { return; }
            playerListMutex.WaitOne();
            foreach (LobbyPlayer w in waitingPlayers)
            {
                if (w.Session == p) { waitingPlayers.Remove(w); break; }
            }
            playerListMutex.ReleaseMutex();
        }




        public String toString()
        {
            if (this.isOver)
            {
                return "\n" + this.Name +
                  "\n" + this.gameType.ToString() +
                  "\n" + getNumberOfPlayers() + "/" + this.PlayerLimit +
                  "\nIN_GAME";
            }
            return "\n" + this.Name +
                   "\n" + this.gameType.ToString() +
                   "\n" + getNumberOfPlayers() + "/" + this.PlayerLimit +
                   "\nWAITING_FOR_PLAYERS";
        }
        private bool CheckIfAlreadyJoined(Session p)
        {
            playerListMutex.WaitOne();
            foreach (LobbyPlayer w in waitingPlayers) {
                if (w.Session == p) { playerListMutex.ReleaseMutex(); return true; } }
            playerListMutex.ReleaseMutex();
            return false;
        }
        private void CleanUpNotExistingPlayers()
        {
            playerListMutex.WaitOne();
            //clean up the waiting clients list
            foreach (LobbyPlayer w in waitingPlayers)
            {
                if (w.Session == null) { waitingPlayers.Remove(w); }
            }
            playerListMutex.ReleaseMutex();

        }

        private uint getNumberOfPlayers() { return (uint)this.waitingPlayers.Count; }
        private bool allPlayersReady()
        {
            playerListMutex.WaitOne();
            foreach (LobbyPlayer p in waitingPlayers)
            {
                if (!p.Ready) { playerListMutex.ReleaseMutex(); return false; }
            }
            playerListMutex.ReleaseMutex();
            return true;
        }
        public void ToogleReady(Session p)
        {
            playerListMutex.WaitOne();
            for(int i=0;i< waitingPlayers.Count; i++)
            {
                if (waitingPlayers[i].Session.id == p.id)
                {
                    waitingPlayers[i].ready = !waitingPlayers[i].ready;
                    break;
                }
            }
                
            
            playerListMutex.ReleaseMutex();
        }
        private List<Session> getAllSessions()
        {
            List<Session> sessions = new List<Session>();
            playerListMutex.WaitOne();
            foreach(LobbyPlayer s in waitingPlayers)
            {
                sessions.Add(s.Session);
            }
            
            playerListMutex.ReleaseMutex();
            return sessions;
        }

        public Game.GAME_TYPE getGameType(){ return this.gameType; }
    
        

    }
}

