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
            Debug.Log("NetworkManagerScp::OnClientConnected being invoked.");
            base.OnClientConnect();
            OnClientJoin?.Invoke();
        }
        
        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
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

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            NetworkPlayerLobby ply = roomPlayer.GetComponent<NetworkPlayerLobby>();
            gamePlayer.GetComponent<NetworkGamePlayer>().role = ply.assignedRole;
            gamePlayer.GetComponent<NetworkGamePlayer>().playerId = ply.id;
            gamePlayer.GetComponent<NetworkGamePlayer>().playerName = ply.playerName;
            PlayerManager.Instance.TryAddPlayer(roomPlayer.GetComponent<NetworkPlayerLobby>(), gamePlayer.GetComponent<NetworkGamePlayer>());
            return true;
        }

        public override void OnRoomServerDisconnect(NetworkConnection connection)
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
        
        public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
        {
            NetworkPlayerLobby room = roomPlayer.GetComponent<NetworkPlayerLobby>();
            Transform startPos =
                SpawnManager.Instance.GetRandomSpawn(room.assignedRole);
            //change this to instantiate a different prefab based on the role
            GameObject gamePlayer = Instantiate(playerPrefab, startPos.position, Quaternion.identity);
            return gamePlayer;
        }
        
        public override void OnServerConnect(NetworkConnection conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }
            
            OnRoomServerConnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            Debug.Log("NetworkManagerScp::OnServerAddPlayer is being called!");
            if (IsSceneActive(RoomScene))
            {
                if (roomSlots.Count == maxConnections)
                    return;

                allPlayersReady = false;

                //Debug.Log("NetworkRoomManager.OnServerAddPlayer playerPrefab: {roomPlayerPrefab.name}");

                GameObject newRoomGameObject = OnRoomServerCreateRoomPlayer(conn);
                if (newRoomGameObject == null)
                    newRoomGameObject = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);

                NetworkServer.AddPlayerForConnection(conn, newRoomGameObject);
            }
            else
            {
                GameObject newRoomGameObject = OnRoomServerCreateRoomPlayer(conn);
                NetworkServer.AddPlayerForConnection(conn, newRoomGameObject);
                GameObject spectator = CreateSpectator(newRoomGameObject.GetComponent<NetworkPlayerLobby>());
                NetworkServer.ReplacePlayerForConnection(conn, spectator);
                PlayerManager.Instance.TryAddPlayer(newRoomGameObject.GetComponent<NetworkPlayerLobby>(), spectator.GetComponent<NetworkGamePlayer>());
            }
        }

        public GameObject CreateSpectator(NetworkPlayerLobby roomPlayer)
        {
            GameObject spectator = Instantiate(deadPlayerPrefab, Vector3.zero, Quaternion.identity);
            NetworkGamePlayer player = spectator.GetComponent<NetworkGamePlayer>();
            spectator.transform.position = SpawnManager.Instance.GetRandomSpawn(player.role).position;
            player.playerName = roomPlayer.playerName;
            player.playerId = roomPlayer.id;
            player.role = roomPlayer.assignedRole;
            player.isSpectating = true;
            return spectator;
        }
    }
}
