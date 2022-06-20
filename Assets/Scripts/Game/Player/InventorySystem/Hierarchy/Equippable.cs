using UnityEngine;

namespace Game.Player.InventorySystem.Items
{
    public class EquippableScriptable : ItemScriptable
    {
        /// <summary>
        /// The number in seconds that it takes to equip this <see cref="EquippableItem"/>
        /// </summary>
        public float EquipTime;

        public virtual void Equipped(ref bool allowed) { }
    }
}