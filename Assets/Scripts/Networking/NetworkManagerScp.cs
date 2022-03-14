using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.GameLogic;
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

            SpawnType spawn = ply.assignedRole == Role.Mtf ? SpawnType.Mtf : SpawnType.Chaos;


            Vector3 position = SpawnManager.Instance.GetRandomSpawn(spawn).position;

            gamePlayer.transform.position = position;
            
            GameManager.Instance.ServerAddPlayer(gamePlayer.GetComponent<NetworkGamePlayer>());
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
    }
}
