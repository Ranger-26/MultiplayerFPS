using System;
using Game.GameLogic;
using Game.ItemSystem.Core;
using Mirror;
using UnityEngine;

namespace Game.Player.InventorySystem
{
    public class Inventory : NetworkBehaviour
    {
        private readonly SyncList<ItemType> _curItems;

        private NetworkGamePlayer player;
        private void Start()
        {
            player = GetComponent<NetworkGamePlayer>();
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
        }

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
                NetworkGamePlayer owner = GameManager.Instance.GetPlayerById(playerId);
                Instantiate(model, owner.transform.position, Quaternion.identity);
            }
        }
    }
}