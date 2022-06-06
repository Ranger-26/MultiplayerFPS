using System;
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

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            NetworkPlayerLobby ply = roomPlayer.GetComponent<NetworkPlayerLobby>();
            gamePlayer.GetComponent<NetworkGamePlayer>().role = ply.assignedRole;
            gamePlayer.GetComponent<NetworkGamePlayer>().playerId = ply.id;
            gamePlayer.GetComponent<NetworkGamePlayer>().playerName = ply.playerName;
            PlayerManager.Instance.TryAddPlayer(roomPlayer.GetComponent<NetworkPlayerLobby>(), gamePlayer.GetComponent<NetworkGamePlayer>());
            return true;
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
            Transform startPos =
                SpawnManager.Instance.GetRandomSpawn(room.assignedRole);
            //change this to instantiate a different prefab based on the role
            GameObject gamePlayer = Instantiate(playerPrefab, startPos.position, Quaternion.identity);
            return gamePlayer;
        }
    }
}
