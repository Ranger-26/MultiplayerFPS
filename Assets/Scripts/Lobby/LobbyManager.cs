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
    }
}