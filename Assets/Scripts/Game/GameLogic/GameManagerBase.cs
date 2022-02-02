using System.Collections.Generic;
using Game.Player;
using Mirror;

namespace Game.GameLogic
{
    public abstract class GameManagerBase : NetworkBehaviour
    {
        protected List<NetworkGamePlayer> players = new List<NetworkGamePlayer>();
                                                                                             
        [Server]
        public virtual void SetUpPlayer(NetworkGamePlayer player) {players.Add(player);}
        
        [Server]
        public virtual void KillPlayer(NetworkGamePlayer player) {players.Remove(player);}

        [Server]
        public virtual bool CheckGameEnd() { return false; }
    }

    public enum GameMode
    {
        Demolition
    }
}