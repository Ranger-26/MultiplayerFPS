using UnityEngine;

namespace Game.GameLogic.ItemSystem.Core
{
    [CreateAssetMenu(fileName = "New Usable Item", menuName = "Items/Usable")]
    public class UsableItemData : ScriptableItemBase
    {
        public float Usetime;
    }
}