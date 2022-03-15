using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!isServer) return;
            Debug.Log("Toggle player is being invoked.");
            if (ply.kickPlayerButton.gameObject.activeSelf) ply.kickPlayerButton.gameObject.SetActive(false);
            if (isServer && ply.player != null && ply.player.id != 1) ply.kickPlayerButton.gameObject.SetActive(!ply.kickPlayerButton.gameObject.activeSelf);
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
                player.CmdChangeReadyState(!player.readyToBegin);
            }
        }

        public void UpdateReadyStatus(int playerId, bool status)
        {
            List<LobbyPlayerUi> players =
                FindObjectsOfType<LobbyPlayerUi>().Where(x => x.player != null && x.player.id == playerId).ToList();
            if (players[0] == null)
            {
                Debug.Log("Uh oh");
            }
            else
            {
                players[0].readyImage.gameObject.SetActive(status);
            }
        }
    }
}