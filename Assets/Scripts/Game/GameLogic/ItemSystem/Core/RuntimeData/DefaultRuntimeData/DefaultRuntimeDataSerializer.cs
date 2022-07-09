using Game.GameLogic.ItemSystem.Core.RuntimeData.Serialization;
using Mirror;

namespace Game.GameLogic.ItemSystem.Core.RuntimeData.DefaultRuntimeData
{
    public static class DefaultRuntimeDataSerializer
    {
        public static IRuntimeData ReadRuntimeData(NetworkReader reader)
        {
            return new DefaultRuntimeData((ItemIdentifier)reader.ReadByte());
        }

        public static void WriteRuntimeData(IRuntimeData data, NetworkWriter writer)
        {
            writer.Write(data.ItemIdentifier);
        }
    }
}