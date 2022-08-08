using System;
using Inputs;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using Networking;
using Lobby;

namespace Game.UI.PlayerList
{
    public class PlayerList : MonoBehaviour
    {
        public bool IsOpen { get; private set; }
        [SerializeField] 
        private TMP_Text _playerListText;
        [SerializeField]
        private GameObject _playerListCanvas;

        private void Start()
        {
            GameInputManager.PlayerActions.PlayerList.performed += ToggleList;
            GameInputManager.PlayerActions.PlayerList.canceled += ToggleList;
        }

        private void OnDestroy()
        {
            GameInputManager.PlayerActions.PlayerList.performed -= ToggleList;
            GameInputManager.PlayerActions.PlayerList.canceled -= ToggleList;
        }

        private void ToggleList(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
                ShowList();
            else if (callbackContext.canceled)
                HideList();
        }

        private void ShowList()
        {
            IsOpen = true;
            _playerListCanvas.SetActive(true);
            _playerListText.text = string.Empty;

            foreach (var name in GetAllPlayerNames())
                _playerListText.text += $"{name}\t\t\tK\tD\tA{Environment.NewLine}";
        }

        private void HideList()
        {
            IsOpen = false;
            _playerListCanvas.SetActive(false);
            //Canvas.ForceUpdateCanvases();
        }

        public string[] GetAllPlayerNames()
        {
            Debug.Log($"Player count: {NetworkManagerScp.Instance.roomSlots.Count}");
            string[] players = new string[NetworkManagerScp.Instance.roomSlots.Count];
            for (int i = 0; i < NetworkManagerScp.Instance.roomSlots.Count; i++)
            {
                NetworkPlayerLobby ply = (NetworkPlayerLobby) NetworkManagerScp.Instance.roomSlots[i];
                players[i] = ply.playerName;
            }
            return players;
        }
    }
}