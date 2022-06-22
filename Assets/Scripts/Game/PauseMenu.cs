using Mirror;
using UnityEngine;
using Networking;

namespace Game
{
    public class PauseMenu : MonoBehaviour
    {
        #region Methods

        public void ReturnToRoom() => NetworkManager.singleton.StopClient();

        #endregion
    }
}