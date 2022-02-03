using Mirror;
using TMPro;
using UnityEngine;

namespace Lobby
{
    public class NetworkLobbyPlayer : NetworkRoomPlayer
    {
        [Header("UI")]
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
        [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
        
        
    }
}