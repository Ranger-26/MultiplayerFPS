using System;
using Game.GameLogic;
using Game.GameLogic.PlayerManagment;
using Mirror;
using Networking;
using UnityEngine;

namespace Lobby
{
    public class NetworkPlayerLobby : NetworkRoomPlayer
    {
        [SyncVar]
        public Role assignedRole;

        [SyncVar] public string playerName = String.Empty;

        [SyncVar] public int id;
        
        private NetworkManagerScp m_room;
        private NetworkManagerScp Room
        {
            get
            {
                if (m_room != null) { return m_room; }
                return m_room = NetworkManager.singleton as NetworkManagerScp;
            }
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            CmdSetName(PlayerProfileManager.GetPlayerName());
        }

        [Command]
        private void CmdSetName(string name)
        {
            playerName = name;
            id = Room.roomSlots.Count + 1;
        }

        public override string ToString()
        {
            return $"Name {playerName}, Id: {id}";
        }
        
        [Command]
        public void CmdReadyUp(bool readyState)
        {
            readyToBegin = readyState;
            NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
            if (room != null)
            {
                room.ReadyStatusChanged();
            }
            //RpcReadyUp(readyState, id);
        }

        [ClientRpc]
        public void RpcReadyUp(bool readyState, int playerid)
        {
            LobbyManager.Instance.UpdateReadyStatus(playerid, readyState);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            Debug.Log($"<color=#FF0000>Player {this} disconnecting..</color>");
            if (PlayerManager.Instance != null)
            {
                PlayerManager.Instance.TryRemovePlayer(this);
            }
        }
    }
}