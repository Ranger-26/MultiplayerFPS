using System.Collections.Generic;
using Lobby;
using Networking;
using UnityEngine;

namespace Game.UI.PlayerList
{
    using System;
    using System.Linq;
    using Inputs;
    using Telepathy;
    using TMPro;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;
    using UnityEngine.UIElements;

    public class PlayerList : MonoBehaviour
    {
        public bool IsOpen { get; private set; }
        [SerializeField] 
        private TextMeshProUGUI _playerListText;
        [SerializeField]
        private Canvas _playerListCanvas;

        private void Start()
        {
            GameInputManager.PlayerActions.PlayerList.performed += ToggleList;
        }

        private void OnDestroy()
        {
            GameInputManager.PlayerActions.PlayerList.performed -= ToggleList;
        }

        private void ToggleList(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed && !IsOpen)
                ShowList();
            else
                HideList();
        }

        private void ShowList()
        {
            IsOpen = true;
            _playerListCanvas.gameObject.SetActive(true);
            _playerListText.text = String.Empty;
            foreach (var name in GetAllPlayerNames())
            {
                _playerListText.text += $"{name}{Environment.NewLine}";
            }
        }

        private void HideList()
        {
            IsOpen = false;
            _playerListCanvas.gameObject.SetActive(false);
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