using Game.GameLogic.ItemSystem.Inventory;
using Mirror;
using Networking;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Consumables.GenericConsumables
{
    public static class GenericConsumableMessageHandler
    {
        public static void OnReceiveMessage(NetworkConnection conn, GenericConsumableMessage message)
        {
            PlayerInventory plr = conn.identity.GetComponent<PlayerInventory>();
            if (plr.CurrentItemBase is IConsumable consumable)
            {
                consumable.ServerOnConsume();
            }
        }

        public static void Register()
        {
            NetworkServer.ReplaceHandler<GenericConsumableMessage>(OnReceiveMessage);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init()
        {
            NetworkManagerScp.OnClientJoin += Register;
        }
    }
}