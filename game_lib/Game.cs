using server_lib;
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

        private List<Session> players;
        private Game_state state;
        private Lobby lobby;

        #region field_definitions
        public List<Session> Players
        {
            get => players;
            set { players = value; }
        }
        public Game_state State
        {
            get => state;
            set { state = value; }
        }


        #endregion

        public Game(uint playerLimit,ulong waitingTime)
        {
            state = Game_state.LOADING_GAME;
            //start the lobby
            this.StartLobby(playerLimit, waitingTime);
        }

        private void StartLobby(uint playerLimit, ulong waitingTime)
        {
            this.lobby = new Lobby(playerLimit, waitingTime);
            this.State = Game_state.LOBBY;
        }

        private void StopLobby()
        {
            //transfer all players from lobby to the game list
            this.players = lobby.GetAllPlayers();
            this.State = Game_state.IN_GAME;
        }


        public bool AddPlayer(Session p)
        {
            if (this.State == Game_state.LOBBY) { lobby.AddPlayer(p); return true;}
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
            if (this.State == Game_state.LOBBY)
            {
                bool end=lobby.update(deltaTime);
                if (end) { StopLobby(); }
            }
            if (this.State == Game.Game_state.IN_GAME || this.State == Game.Game_state.AFTER_GAME) { updateGame(deltaTime);}
        }

     

        public abstract void updateGame(ulong deltaTime);
        public abstract void StartGame();
        public abstract void StopGame();
    }
}
