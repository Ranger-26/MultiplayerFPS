using UnityEngine;

namespace Game.Player.InventorySystem.Items
{
    public class ItemBase : MonoBehaviour
    {
        public virtual string DisplayName { get; set; }
        public virtual ItemHandlers.ItemType Type { get; set; }
        public virtual ItemHandlers.ItemCategoryType Category { get; set; }
        public virtual byte CertsRequired { get; set; }
        public virtual float Weight { get; set; }
        public virtual bool Droppable { get; set; } = true;

        public virtual void Dropped(bool byDeath, ref bool allowed) {}
        public virtual void Bought(ref bool allowed) {}
    }
}