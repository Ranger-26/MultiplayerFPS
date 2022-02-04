using System;
using Game.GameLogic;
using Mirror;
using Networking;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class NetworkPlayerLobby : NetworkRoomPlayer
    {
        [SyncVar]
        public Role assignedRole;

        [SyncVar] public string playerName; 
        
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
            CmdSetName();
        }

        
        [Command]
        private void CmdSetName()
        {
            playerName = "Player" + Room.allPlayers.Count;
        }
    }
}