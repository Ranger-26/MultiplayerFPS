using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class GamePlayerInput : MonoBehaviour
    {
        public static GamePlayerInput Instance;

        [HideInInspector]
        public PlayerInput playerInput;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            playerInput = GetComponent<PlayerInput>();

            DontDestroyOnLoad(gameObject);
        }
    }
}