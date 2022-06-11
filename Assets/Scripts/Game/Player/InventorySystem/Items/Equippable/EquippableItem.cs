using UnityEditor;

namespace Game.Player.InventorySystem.Items.Equippable
{
    public class EquippableItem : ItemBase
    {
        /// <summary>
        /// The number in seconds that it takes to equip this <see cref="EquippableItem"/>
        /// </summary>
        public virtual float EquipTime { get; set; }

        public virtual void Equipped(ref bool allowed) {}
    }
}