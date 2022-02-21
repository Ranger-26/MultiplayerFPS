using System;
using System.Collections.Generic;
using System.Linq;
using Game.Player;
using Mirror;
using UnityEngine;

namespace Game.GameLogic
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField]
        private List<NetworkGamePlayer> players = new List<NetworkGamePlayer>();

        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        [Server]
        public void ServerAddPlayer(NetworkGamePlayer ply)
        {
            players.Add(ply);
        }

        public NetworkGamePlayer GetPlayerById(int id) => players.Where(x => x.playerId == id).ToList()[0];
    }
}