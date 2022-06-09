using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Player;
using Lobby;
using Mirror;
using Networking;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.GameLogic.PlayerManagment
{
    public class PlayerManager : NetworkBehaviour
    {
        [SerializeField]
        private List<NetworkPlayerLobby> lobbyPlayers = new List<NetworkPlayerLobby>();
        
        public Dictionary<NetworkPlayerLobby, NetworkGamePlayer> players { get; } = new Dictionary<NetworkPlayerLobby, NetworkGamePlayer>();

        public static PlayerManager Instance;

        [SerializeField]
        private List<NetworkGamePlayer> alivePlayers = new List<NetworkGamePlayer>();
        
        [SerializeField]
        private List<NetworkGamePlayer> spectators = new List<NetworkGamePlayer>();
        

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        

        [Server]
        public void TryAddPlayer(NetworkPlayerLobby playerOrigin, NetworkGamePlayer currentPlayer)
        {
            if (players.ContainsKey(playerOrigin) || players.ContainsValue(currentPlayer))
            {
                Debug.LogError($"Something went wrong when adding Player {currentPlayer}.");
                return;
            }
            lobbyPlayers.Add(playerOrigin);
            players.Add(playerOrigin, currentPlayer);
            alivePlayers.Add(currentPlayer);
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

            if (players[playerOrigin].isSpectating)
            {
                spectators.Remove(players[playerOrigin]);
            }
            else
            {
                alivePlayers.Remove(players[playerOrigin]);
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
                Debug.LogError($"Could not find player {playerOrigin} in dictionary!");
                return;
            }
            
            players.Remove(player);
            alivePlayers.Remove(playerOrigin);
            spectators.Add(newPlayer);
            players.Add(player, newPlayer);
            Debug.Log($"Game Player Count: {players.Count}");

            if (GameEnded)
            {
                StartCoroutine(GameManager.Instance.RestartRound());
            }
        }

        
        
        public bool GameEnded => alivePlayers.Count <= 1;
        
        [Server]
        public void ResetPlayers(Dictionary<NetworkPlayerLobby, NetworkGamePlayer> newPlayers)
        {
            lobbyPlayers.Clear();
            players.Clear();
            alivePlayers.Clear();
            spectators.Clear();
            players.AddRange(newPlayers);
            lobbyPlayers.AddRange(players.Keys);
            alivePlayers.AddRange(players.Values);
        }
    }
}