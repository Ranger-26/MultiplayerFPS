using Mirror;
using UnityEngine;

namespace Lobby
{
    public class LobbyManager : NetworkBehaviour
    {
        [Server]
        public void KickPlayer(LobbyPlayerUi ply)
        {
            Debug.Log($"Trying to kick player {ply.player.playerName}");
            ply.player.GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }
        
        public void ToggleKickPlayer(LobbyPlayerUi ply)
        {
            if (isServer) ply.kickPlayerButton.gameObject.SetActive(!ply.kickPlayerButton.gameObject.activeSelf);
        }

        public void LeaveGame()
        {
            if (isServer) NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopClient();
        }
    }
}