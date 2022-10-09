using Game.GameLogic.ItemSystem.Inventory;
using Game.GameLogic.ItemSystem.Items.Knife;
using Mirror;
using Networking;
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
                Debug.Log("Using current usable held item...");
                item.OnServerReceiveUseMessage();
                plr.ServerDestroyHeldItem();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            NetworkManagerScp.OnClientJoin += delegate {NetworkServer.ReplaceHandler<UseUsableItemMessage>(OnReceiveMessage);  }; 
        }
    }
}