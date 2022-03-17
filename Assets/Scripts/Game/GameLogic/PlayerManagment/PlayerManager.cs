using System;
using System.Collections.Generic;
using System.Linq;
using Game.Player;
using Lobby;
using Mirror;
using Networking;
using UnityEngine;

namespace Game.GameLogic.PlayerManagment
{
    public class PlayerManager : NetworkBehaviour
    {
        [SerializeField]
        private List<NetworkPlayerLobby> lobbyPlayers = new List<NetworkPlayerLobby>();
        
        private Dictionary<NetworkPlayerLobby, NetworkGamePlayer> players =
            new Dictionary<NetworkPlayerLobby, NetworkGamePlayer>();

        public static PlayerManager Instance;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private void Init()
        {
            Instance = null;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            lobbyPlayers.AddRange(NetworkManagerScp.Instance.allPlayers);
        }

        [Server]
        public void TryAddPlayer(NetworkPlayerLobby playerOrigin, NetworkGamePlayer currentPlayer)
        {
            if (players.ContainsKey(playerOrigin) || players.ContainsValue(currentPlayer))
            {
                Debug.LogError($"Something went wrong when adding Player {currentPlayer}.");
                return;
            }
            players.Add(playerOrigin, currentPlayer);
            Debug.Log($"Game Player Count: {players.Count}");
        }

        [Server]
        public void TryRemovePlayer(NetworkPlayerLobby playerOrigin)
        {
            if (!players.ContainsKey(playerOrigin) || !lobbyPlayers.Contains(playerOrigin))
            {
                Debug.LogError($"Something went wrong when removing Player {playerOrigin.id}.");
                return;
            }

            players.Remove(playerOrigin);
            
            lobbyPlayers.Remove(playerOrigin);
            Debug.Log($"Game Player Count: {players.Count}");
        }

        [Server]
        public void TryHandleDeadPlayer(NetworkGamePlayer playerOrigin, NetworkGamePlayer newPlayer)
        {
            if (!players.ContainsValue(playerOrigin))
            {
                Debug.LogError($"Something went wrong when adding Player {playerOrigin.playerId}.");
                return;
            }

            NetworkPlayerLobby player = null;

            foreach (var pair in players)
            {
                if (pair.Value == playerOrigin)
                {
                    player = pair.Key;
                }
            }

            if (player == null)
            {
                Debug.LogError($"Could not find player {playerOrigin} in dicionary!");
                return;
            }
            
            players.Remove(player);
            players.Add(player, newPlayer);
            Debug.Log($"Game Player Count: {players.Count}");
        }
        
        
    }
}