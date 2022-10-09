using UnityEngine;

namespace Game.GameLogic.ItemSystem.Core.UsableItems
{
    [CreateAssetMenu(fileName = "New Usable Item", menuName = "Items/Usable")]
    public class UsableItemData : ScriptableItemBase
    {
        public float Usetime;
    }
}