using Mirror;

namespace Game.GameLogic.ItemSystem.Core.UsableItems
{
    public struct UseUsableItemMessage : NetworkMessage
    {
        public ItemIdentifier item;
    }
}