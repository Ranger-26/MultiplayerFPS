using System;
using System.Collections;
using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AudioUtils;
using Game.GameLogic;
using Game.GameLogic.PlayerManagment;
using Game.GameLogic.Spawning;
using Game.Player;
using Lobby;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Networking
{
    public class NetworkManagerScp : NetworkRoomManager
    {
        public List<NetworkPlayerLobby> allPlayers = new List<NetworkPlayerLobby>();
        
        public List<NetworkPlayerLobby> mtf = new List<NetworkPlayerLobby>();

        public List<NetworkPlayerLobby> chaos = new List<NetworkPlayerLobby>();

        public GameObject deadPlayerPrefab;

        public GameObject ragDoll;

        public static event Action OnClientJoin;
        
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            OnClientJoin?.Invoke();
            
            var spawnablePrefabs = Resources.LoadAll<GameObject>("Prefabs/SpawnablePrefabs");

            foreach (var prefab in spawnablePrefabs)
            {
                if (prefab.TryGetComponent(out NetworkIdentity identity))
                {
                    NetworkClient.RegisterPrefab(prefab);
                }
            }
        }
        
        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
        {
            GameObject roomPlayer = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
            allPlayers.Add(roomPlayer.GetComponent<NetworkPlayerLobby>());
            SetPlayerRole(roomPlayer.GetComponent<NetworkPlayerLobby>());
            return roomPlayer;
        }

        [Server]
        private void SetPlayerRole(NetworkPlayerLobby player)
        {
            if (chaos.Count == mtf.Count)
            {
                player.assignedRole = Role.Chaos;
                chaos.Add(player);
            }
            else if (chaos.Count > mtf.Count)
            {
                player.assignedRole = Role.Mtf;
                mtf.Add(player);
            }
            else
            {
                player.assignedRole = Role.Chaos;
                chaos.Add(player);
            }
        }

        public override void OnRoomServerDisconnect(NetworkConnectionToClient connection)
        {
            if (connection.identity.TryGetComponent(out NetworkPlayerLobby player))
            {
                if (player.assignedRole == Role.Mtf)
                {
                    mtf.Remove(player);
                }
                if (player.assignedRole == Role.Chaos)
                {
                    chaos.Remove(player);
                }
            }
        }

        public static NetworkManagerScp Instance => (NetworkManagerScp) singleton;
        
        public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            NetworkPlayerLobby room = roomPlayer.GetComponent<NetworkPlayerLobby>();
            Vector3 startPos =
                SpawnManager.Instance.GetRandomSpawn(room.assignedRole);
            GameObject gamePlayer = Instantiate(playerPrefab, startPos, Quaternion.identity);
            StartCoroutine(SetUpGamePlayer(room, gamePlayer.GetComponent<NetworkGamePlayer>()));
            return gamePlayer;
        }

        IEnumerator SetUpGamePlayer(NetworkPlayerLobby lobby, NetworkGamePlayer game)
        {
            yield return new WaitUntil(() => lobby.playerName != "" && lobby.playerId != -1);
            game.GetComponent<NetworkGamePlayer>().playerId = lobby.playerId;
            game.GetComponent<NetworkGamePlayer>().playerName = lobby.playerName;
            game.role = lobby.assignedRole;
            PlayerManager.Instance.TryAddPlayer(lobby, game);
        }
    }
}
