using Game.ItemSystem.Core;
using Mirror;
using UnityEngine;

namespace Game.Player.InventorySystem
{
    public class Inventory : NetworkBehaviour
    {
        private readonly SyncList<ItemType> _curItems;

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

        private void ServerEquiptItem(int index)
        {
            if (_curItems[index] == 0) return;
            _curItem = _curItems[index];
        }

        [ClientRpc]
        public void SyncItem(ItemIdentifier item, string playerName)
        {
            
        }
    }
}