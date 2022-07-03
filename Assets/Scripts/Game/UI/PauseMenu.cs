using Mirror;
using UnityEngine;

namespace Game.UI
{
    public class PauseMenu : MonoBehaviour
    {
        #region Methods

        public void ReturnToRoom() => NetworkManager.singleton.StopClient();

        #endregion
    }
}