using UnityEngine;

namespace Game.Player.InventorySystem.Items
{
    public class ItemScriptable : ScriptableObject
    {
        public string DisplayName;
        public ItemHandlers.ItemType Type;
        public ItemHandlers.ItemCategoryType Category;
        public byte CertsRequired;
        public float Weight;
        public bool Droppable = true;

        public virtual void Dropped(bool byDeath, ref bool allowed) { }
        public virtual void Bought(ref bool allowed) { }
    }
}