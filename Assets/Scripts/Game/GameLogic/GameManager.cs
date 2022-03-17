using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameLogic.Spawning;
using Game.Player;
using Lobby;
using Mirror;
using Networking;
using UnityEngine;

namespace Game.GameLogic
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        #region HelperFunctions
        private GameObject CreateNewPlayer(Role role, NetworkGamePlayer ply)
        {
            GameObject player = Instantiate(NetworkManagerScp.Instance.playerPrefab,
                SpawnManager.Instance.GetRandomSpawn(role).position, Quaternion.identity);
            NetworkGamePlayer playerNew = player.GetComponent<NetworkGamePlayer>();
            playerNew.playerName = ply.playerName;
            playerNew.playerId = ply.playerId;
            return player;
        }
        #endregion

        public Dictionary<NetworkPlayerLobby, NetworkGamePlayer> RespawnAllPlayers(Dictionary<NetworkPlayerLobby, NetworkGamePlayer> players)
        {
            Dictionary<NetworkPlayerLobby, NetworkGamePlayer> newDict =
                new Dictionary<NetworkPlayerLobby, NetworkGamePlayer>();
            foreach (var player in players)
            {
                GameObject newPlayer = CreateNewPlayer(player.Value.role, player.Value);
                NetworkServer.ReplacePlayerForConnection(player.Value.netIdentity.connectionToClient, newPlayer);
                Destroy(player.Value.gameObject);
                newDict.Add(player.Key, newPlayer.GetComponent<NetworkGamePlayer>());
            }

            return newDict;
        }
    }
}