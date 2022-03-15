using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{
    public class GameUiManager : MonoBehaviour
    {
        [SerializeField]
        private Text _healthText;
        [SerializeField]
        private GameObject Menu;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Menu.SetActive(true);
            }
        }

        public void UpdateHealthUI(int newHealth) => _healthText.text = newHealth.ToString();
    }
}