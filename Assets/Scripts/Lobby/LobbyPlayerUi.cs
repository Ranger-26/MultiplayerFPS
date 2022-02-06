using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyPlayerUi : MonoBehaviour
    {
        public NetworkPlayerLobby player;

        public Button kickPlayerButton;

        public Image readyImage;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(()=>LobbyManager.Instance.ToggleKickPlayer(this));
            kickPlayerButton.onClick.AddListener(()=>LobbyManager.Instance.KickPlayer(this));
        }
    }
}