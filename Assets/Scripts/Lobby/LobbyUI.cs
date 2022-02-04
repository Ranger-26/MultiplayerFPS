using System;
using System.Collections;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUI : NetworkBehaviour
    {
        public static LobbyUI Instance;

        private void Awake()
        {
            if (Instance != null) Destroy(this);
            Instance = this;
        }

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
        private Text[] nameTexts = new Text[5];
    }
}