using System;
using System.Linq;
using Game.GameLogic;
using Game.ItemSystem.Core;
using Mirror;
using UnityEngine;

namespace Game.Player.InventorySystem
{
    public class Inventory : NetworkBehaviour
    {
        private readonly SyncList<ItemType> _curItems = new SyncList<ItemType>();

        private NetworkGamePlayer player;
        private void Start()
        {
            player = GetComponent<NetworkGamePlayer>();

            if (hasAuthority)
            {
                CmdAddItem(ItemType.Medkit);
            }
        }

        [SyncVar]
        private ItemType _curItem = ItemType.None;

        [Server]
        public void ServerAddItem(ItemType item)
        {
            if (_curItems.Count < 8)
            {
                _curItems.Add(item);
            }
            TargetPrintOutItems();
        }
        
        
        [Command]
        public void CmdAddItem(ItemType item) => ServerAddItem(item);
        
        [Command]
        public void CmdEquiptItem(int index) => ServerEquiptItem(index);

        
        
        [Server]
        private void ServerEquiptItem(int index)
        {
            if (_curItems[index] == 0) return;
            _curItem = _curItems[index];
            RpcEqupitItem(_curItem, player.playerId);
        }

        [ClientRpc]
        public void RpcEqupitItem(ItemType item, int playerId)
        {
            if (ItemDatabase.Instance.TryGetItem(item, out ItemViewModel model))
            {
                NetworkGamePlayer owner = GetPlayerById(playerId);
                Debug.Log($"Null Check Player: {owner == null}");
                Debug.Log($"Null Check Item: {model == null}");
                Instantiate(model.gameObject, owner.transform.position, Quaternion.identity);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && hasAuthority)
            {
                CmdEquiptItem(0);
            }
        }

        [TargetRpc]
        private void TargetPrintOutItems()
        {
            for (int i = 0; i < _curItems.Count; i++)
            {
               Debug.Log($"Item: {_curItems[i]}"); 
            }
        }

        public NetworkGamePlayer GetPlayerById(int id)
        {
            NetworkGamePlayer[] players = FindObjectsOfType<NetworkGamePlayer>();
            return players.Where(x => x.playerId == id).ToList()[0];
        }
    }
}