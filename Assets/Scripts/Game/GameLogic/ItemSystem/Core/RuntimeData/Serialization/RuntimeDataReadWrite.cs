using System;
using System.Collections.Generic;
using System.Reflection;
using Game.GameLogic.ItemSystem.Core.RuntimeData.DefaultRuntimeData;
using Mirror;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Core.RuntimeData.Serialization
{
    public static class RuntimeDataReadWrite
    {
        public static Dictionary<ItemIdentifier, CustomSerializerBase> idsToSerializers = new();
        
        public static void WriteRuntimeData(this NetworkWriter writer, IRuntimeData data)
        {
            writer.WriteByte((byte)data.ItemIdentifier);
            if (idsToSerializers.ContainsKey(data.ItemIdentifier))
            {
                idsToSerializers[data.ItemIdentifier].WriteRuntimeData(data, writer);
            }
            else
            {
                DefaultRuntimeDataSerializer.WriteRuntimeData(data, writer);
            }
        }

        public static IRuntimeData ReadRuntimeData(this NetworkReader reader)
        {
            ItemIdentifier item = (ItemIdentifier)reader.ReadByte();
            if (idsToSerializers.ContainsKey(item))
            {
                return idsToSerializers[item].ReadRuntimeData(reader);
            }

            return new DefaultRuntimeData.DefaultRuntimeData(item);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init()
        {
            foreach (var type in Assembly.GetExecutingAssembly().DefinedTypes)
            {
                if (type.BaseType == typeof(CustomSerializerBase))
                {
                    CustomSerializerBase serializer = (CustomSerializerBase)type.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
                    idsToSerializers.Add(serializer.ItemIdentifier, serializer);
                }
            }
        }
    }
}