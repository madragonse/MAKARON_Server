using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_lib
{
    public abstract class Game
    {
        public enum Game_state
        {
            LOADING_GAME,
            LOBBY,
            IN_GAME,
            AFTER_GAME
        }
        public enum GameName
        {
            BOMBERMAN,
            SOME_OTHER_GAME
        }

        private List<Session> sessions;
        private Game_state state;
        private Lobby lobby;
        private String name;

        #region field_definitions
        public List<Session> Sessions
        {
            get => sessions;
            set { sessions = value; }
        }
        public Game_state State
        {
            get => state;
            set { state = value; }
        }
        public String Name
        {
            get => name;
            set { name = value; }
        }


        #endregion

        public Game(uint playerLimit,ulong waitingTime, String name)
        {
            state = Game_state.LOADING_GAME;
            //start the lobby
            //TODO start game
            this.Sessions = new List<Session>();
            this.Name = name;
        }



        public bool AddPlayer(Session p)
        {
            if (this.State == Game_state.LOBBY) {
                return lobby.AddPlayer(p);
            }
            //
            return false;
        }
        public bool RemovePlayer(Session p)
        {
            if (this.State == Game_state.LOBBY) { lobby.RemovePlayer(p); return true; }
            //
            return false;
        }

        public void update(ulong deltaTime)
        {

        }


        public override String ToString()
        {
            return  "\n\rLobbyName: "+this.Name+ 
                    "\n\rGameType: "+ getGameType()+ 
                    "\n\rPlayers: " + getNumberOfPlayers()+"/"+this.lobby.PlayerLimit+ 
                    "\n\rState: " + this.state.ToString(); 
        }

        public String[] getData()
        {
            String [] result = { this.Name,
                                 getGameType().ToString(),
                                 getNumberOfPlayers().ToString(),
                                 this.lobby.PlayerLimit.ToString(),
                                 this.state.ToString() };
            return result;
        }
           


        private uint getNumberOfPlayers()
        {
            if (this.State == Game_state.LOBBY) { return (uint)this.lobby.Waiting.Count; }
            return (uint) this.Sessions.Count;
        }

        public abstract void updateGame(ulong deltaTime);
        public abstract void StartGame();
        public abstract void StopGame();
        public abstract GameName getGameType();
    }
}
