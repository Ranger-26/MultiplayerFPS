using System;
using Mirror;
using UnityEngine;

namespace Lobby
{
    public class LobbyManager : NetworkBehaviour
    {
        public static LobbyManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        [Server]
        public void KickPlayer(LobbyPlayerUi ply)
        {
            Debug.Log($"Trying to kick player {ply.player.playerName}");
            ply.player.GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }
        
        public void ToggleKickPlayer(LobbyPlayerUi ply)
        {
            if (isServer && ply.player != null) ply.kickPlayerButton.gameObject.SetActive(!ply.kickPlayerButton.gameObject.activeSelf);
        }

        public void LeaveGame()
        {
            if (isServer) NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopClient();
        }

        public void ReadyUp()
        {
            foreach (var player in FindObjectsOfType<NetworkPlayerLobby>())
            {
                player.CmdReadyUp(!player.readyToBegin);
            }
        }

        public void UpdateReadyStatus(int playerId, bool status)
        {
            foreach (var player in FindObjectsOfType<LobbyPlayerUi>())
            {
                if (player.player != null && player.player.id == playerId)
                {
                    player.readyImage.gameObject.SetActive(status);
                }
            }
        }
    }
}