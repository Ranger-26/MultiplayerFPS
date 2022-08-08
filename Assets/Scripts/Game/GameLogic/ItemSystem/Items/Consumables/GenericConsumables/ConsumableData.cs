using Game.GameLogic.ItemSystem.Core;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Consumables.GenericConsumables
{
    [CreateAssetMenu(fileName = "New Consumable Item", menuName = "Items/Consumable")]
    public class ConsumableData : ScriptableItemBase
    {
        public int UseTime;
    }
}