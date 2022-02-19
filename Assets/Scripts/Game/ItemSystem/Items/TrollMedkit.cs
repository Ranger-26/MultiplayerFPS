using Game.ItemSystem.Core;
using Game.Player;
using Mirror;

namespace Game.ItemSystem.Items
{
    public class TrollMedkit : ItemBase
    {
        public override ItemType ItemId { get; } = ItemType.Medkit;

        private int _healAmount = 20;
        
                                             
        [Server]
        public override void OnUse(NetworkGamePlayer ply)
        {
            ply.healthController.ServerDamagePlayer(_healAmount);
        }
    }
}