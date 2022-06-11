using UnityEngine;

namespace Game
{
    public class EnableCursor : MonoBehaviour
    {
        #region Unity Callbacks

        private void Start() => Cursor.lockState = CursorLockMode.None;
        private void OnEnable() => Cursor.lockState = CursorLockMode.None;

        #endregion
    }
}