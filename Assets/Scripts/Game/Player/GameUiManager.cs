using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{
    public class GameUiManager : MonoBehaviour
    {
        public Text healthText;

        public static GameUiManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Another instance of GameUiManager already exists, destroying...");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
    }
}