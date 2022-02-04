using System;
using System.Collections;
using System.Linq;
using Game.GameLogic;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        private NetworkManagerScp m_room;

        private NetworkManagerScp Room
        {
            get
            {
                if (m_room != null) { return m_room; }
                return m_room = NetworkManager.singleton as NetworkManagerScp;
            }
        }
        
        [SerializeField]
        private Text[] mtfNameTexts = new Text[5];
        
        [SerializeField]
        private Text[] chaosNameTexts = new Text[5];
        private void Update()
        {
            int countMtf = 0;
            int countChaos = 0;
            
            for (int i = 0; i < Room.roomSlots.Count; i++)
            {
                NetworkPlayerLobby player = (NetworkPlayerLobby) Room.roomSlots[i];
                if (player.assignedRole == Role.Mtf)
                {
                    mtfNameTexts[countMtf].text = player.playerName;
                    countMtf++;
                }
                if (player.assignedRole == Role.Chaos)
                {
                    chaosNameTexts[countChaos].text = player.playerName;
                    countChaos++;
                }
            }
        }
    }
}