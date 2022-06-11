using UnityEngine;
using Networking;

namespace Game
{
    public class PauseMenu : MonoBehaviour
    {
        #region Methods

        public void ReturnToRoom() => NetworkManagerScp.Instance.ServerChangeScene("Lobby");

        #endregion
    }
}