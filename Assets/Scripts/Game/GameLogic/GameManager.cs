using System;
using System.Collections.Generic;
using Game.Player;
using Mirror;

namespace Game.GameLogic
{
    public class GameManager : NetworkBehaviour
    {
        private List<NetworkGamePlayer> players = new List<NetworkGamePlayer>();

        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void AddPlayer(NetworkGamePlayer ply)
        {
            
        }
    }
}