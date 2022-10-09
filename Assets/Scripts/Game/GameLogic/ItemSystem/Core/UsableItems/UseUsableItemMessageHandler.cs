using Game.GameLogic.ItemSystem.Inventory;
using Game.GameLogic.ItemSystem.Items.Knife;
using Mirror;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Core.UsableItems
{
    public static class UseUsableItemMessageHandler
    {
        public static void OnReceiveMessage(NetworkConnection conn, UseUsableItemMessage message)
        {
            PlayerInventory plr = conn.identity.GetComponent<PlayerInventory>();
            if (plr.currentItem != message.item)
            {
                Debug.Log("Mismatch in current item, skipping..");
                return;
            }

            if (plr.CurrentItemBase is UsableItemBase)
            {
                UsableItemBase item = (UsableItemBase) plr.CurrentItemBase;
                item.OnServerReceiveUseMessage();
                plr.ServerDestroyHeldItem();
            }
        }
    }
}