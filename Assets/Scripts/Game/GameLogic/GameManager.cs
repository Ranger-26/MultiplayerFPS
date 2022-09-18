using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GameLogic.Map;
using Game.GameLogic.PlayerManagment;
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
                SpawnManager.Instance.GetRandomSpawn(role), Quaternion.identity);
            NetworkGamePlayer playerNew = player.GetComponent<NetworkGamePlayer>();
            playerNew.playerName = ply.playerName;
            playerNew.playerId = ply.playerId;
            playerNew.role = role;
            return player;
        }
        #endregion

        public Dictionary<NetworkPlayerLobby, NetworkGamePlayer> RespawnAllPlayers(Dictionary<NetworkPlayerLobby, NetworkGamePlayer> players)
        {
            Dictionary<NetworkPlayerLobby, NetworkGamePlayer> newDict =
                new Dictionary<NetworkPlayerLobby, NetworkGamePlayer>();
            SpawnManager.Instance.ResetSpawnPoints();
            foreach (var player in players)
            {
                GameObject newPlayer = CreateNewPlayer(player.Value.role, player.Value);
                NetworkServer.ReplacePlayerForConnection(player.Value.netIdentity.connectionToClient, newPlayer);
                Destroy(player.Value.gameObject);
                newDict.Add(player.Key, newPlayer.GetComponent<NetworkGamePlayer>());
            }

            return newDict;
        }
        
        [Server]
        public IEnumerator RestartRound()
        {
            Debug.Log("Restarting round....");

            //respawn time for now can just be 5 seconds
            int respawnTime = 5;
            for (int i = 0; i < respawnTime; i++)
            {
                RpcSendTimer(5-i, false);
                yield return new WaitForSeconds(1);
            }

            RpcSendTimer(0, true);
            MapSpawnableObject.DestroyAllMapObjects();
            PlayerManager.Instance.ResetPlayers(RespawnAllPlayers(PlayerManager.Instance.players));
            

            Debug.Log("New round has started!");
        }

        [ClientRpc]
        private void RpcSendTimer(int newTime, bool disable)
        {
            GameUiManager.Instance.UpdateUiTimer(newTime, disable);
        }
    }
}