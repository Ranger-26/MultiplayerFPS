using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Inventory;
using Game.Player.Damage;
using Mirror;
using Networking;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Consumables.SCP500
{
    public static class Scp500Handler
    {
        private static void OnReceiveMessage(NetworkConnection conn, Scp500Message message)
        {
            PlayerInventory plr = conn.identity.GetComponent<PlayerInventory>();
            if (plr.currentItem == ItemIdentifier.SCP500)
            {
                plr.GetComponent<HealthController>().ServerHealPlayer(100);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init()
        {
            NetworkManagerScp.OnClientJoin += delegate
            {
                NetworkServer.ReplaceHandler<Scp500Message>(OnReceiveMessage);
            };
        }
    }
}