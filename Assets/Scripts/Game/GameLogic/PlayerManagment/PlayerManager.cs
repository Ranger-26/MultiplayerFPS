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

        public void TryAddPlayer(NetworkPlayerLobby playerOrigin, NetworkGamePlayer currentPlayer)
        {
            if (players.ContainsKey(playerOrigin) || players.ContainsValue(currentPlayer))
            {
                Debug.LogError($"Something went wrong when adding Player {currentPlayer}.");
                return;
            }
            players.Add(playerOrigin, currentPlayer);
        }

        public void TryRemovePlayer(NetworkPlayerLobby playerOrigin)
        {
            if (!players.ContainsKey(playerOrigin) || !lobbyPlayers.Contains(playerOrigin))
            {
                Debug.LogError($"Something went wrong when removing Player {playerOrigin.id}.");
                return;
            }

            players.Remove(playerOrigin);
            lobbyPlayers.Remove(playerOrigin);
        }

        public void TryHandleDeadPlayer(NetworkPlayerLobby playerOrigin, NetworkGamePlayer newPlayer)
        {
            if (!players.ContainsKey(playerOrigin))
            {
                Debug.LogError($"Something went wrong when adding Player {playerOrigin.id}.");
                return;
            }

            players.Remove(playerOrigin);
            players.Add(playerOrigin, newPlayer);
        }
    }
}