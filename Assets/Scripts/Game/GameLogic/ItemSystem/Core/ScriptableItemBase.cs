using UnityEngine;

namespace Game.GameLogic.ItemSystem.Core
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items")]
    public class ScriptableItemBase : ScriptableObject
    {
        public ItemIdentifier ItemIdentifier;
        public float ItemDrawTime;

        public Sprite Icon;
    }
}