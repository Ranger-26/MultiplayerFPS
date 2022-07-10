using System;
using Mirror;
using Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Player
{
    public class GameUiManager : MonoBehaviour
    {
        [SerializeField]
        private Text _healthText;
        [SerializeField]
        private Text _ammoText;
        [SerializeField]
        private GameObject Menu;

        [SerializeField]
        private Button _disconnectButton;

        [SerializeField]
        private Text _gameOverText;
        
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
            
            _ammoText.gameObject.SetActive(false);
            
            GameInputManager.PlayerActions.Pause.performed += Pause;
        }

        public void SetAmmoTextState(bool state) => _ammoText.gameObject.SetActive(state);
        
        
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

        private void OnDestroy()
        {
            GameInputManager.PlayerActions.Pause.performed -= Pause;
        }

        public void UpdateHealthUI(int newHealth) => _healthText.text = newHealth.ToString();

        public void UpdateAmmoUI(int currentAmmo, int reserveAmmo) => _ammoText.text = ((currentAmmo >= 200000) ? "inf" : currentAmmo.ToString()) + " / " + ((reserveAmmo >= 200000) ? "inf" : reserveAmmo.ToString());

        public void OnDie()
        {
            _ammoText.gameObject.SetActive(false);
            _healthText.gameObject.SetActive(false);
        }

        public void OnRespawn()
        {
            //_ammoText.gameObject.SetActive(true);
            UpdateHealthUI(100);
            _healthText.gameObject.SetActive(true);
        }
        
        public void UpdateUiTimer(int time, bool shouldDisable = false)
        {
            _gameOverText.gameObject.SetActive(true);
            _gameOverText.text = $"Game over: Restarting in {time}";
            _gameOverText.gameObject.SetActive(!shouldDisable);
        }

        public void Pause(InputAction.CallbackContext callbackContext)
        {
            Menu.SetActive(true);
        }
    }
}