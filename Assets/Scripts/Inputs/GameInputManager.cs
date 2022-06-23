using System;
using UnityEngine;

namespace Inputs
{
    public class GameInputManager : MonoBehaviour
    {
        public static InputActions Actions;

        public static GameInputManager Instance;

        public static InputActions.PlayerActions PlayerActions => Actions.Player;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Actions = new InputActions();
                Actions.Player.Enable();
            }
            else
            {
                Destroy(this);
            }
        }

        public void OnDestroy()
        {
            Actions.Player.Disable();
        }
    }
}