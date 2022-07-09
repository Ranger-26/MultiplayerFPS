namespace Game.GameLogic.ItemSystem.Core.RuntimeData.DefaultRuntimeData
{
    public struct DefaultRuntimeData : IRuntimeData
    {
        public DefaultRuntimeData(ItemIdentifier id)
        {
            ItemIdentifier = id;
        }

        public ItemIdentifier ItemIdentifier { get; }
    }
}