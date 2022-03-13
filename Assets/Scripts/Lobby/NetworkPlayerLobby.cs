using System;
using Game.GameLogic;
using Mirror;
using Networking;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

        public override bool Equals(object other)
        {
            if (other is NetworkPlayerLobby)
            {
                NetworkPlayerLobby plr = (NetworkPlayerLobby) other;
                return plr.id == id;
            }

            return false;
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
    }
}