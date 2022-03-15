using System;
using Mirror;
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

        [SerializeField]
        private Button _disconnectButton;
        
        public static GameUiManager Instance;

        private void Start()
        {
            _disconnectButton.onClick.AddListener(()=>
            {
                if (NetworkGamePlayer.localPlayer.isServer)
                {
                    NetworkManager.singleton.StopHost();
                }
                NetworkManager.singleton.StopClient();
            });
        }


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