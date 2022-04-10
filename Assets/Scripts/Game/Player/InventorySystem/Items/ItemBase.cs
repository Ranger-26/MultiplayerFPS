using UnityEngine;

namespace Game.Player.InventorySystem.Items
{
    public abstract class ItemBase : MonoBehaviour
    {
        public abstract string DisplayName { get; set; }
        public abstract string Description { get; set; }
        public abstract ItemHandlers.ItemType ItemType { get; set; }
        
    }
}