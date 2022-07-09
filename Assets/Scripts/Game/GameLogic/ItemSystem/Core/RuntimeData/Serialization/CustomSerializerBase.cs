using Mirror;

namespace Game.GameLogic.ItemSystem.Core.RuntimeData.Serialization
{
    public abstract class CustomSerializerBase
    {
        public abstract ItemIdentifier ItemIdentifier { get; }

        public abstract IRuntimeData ReadRuntimeData(NetworkReader reader);

        public abstract void WriteRuntimeData(IRuntimeData data, NetworkWriter writer);
    }
}