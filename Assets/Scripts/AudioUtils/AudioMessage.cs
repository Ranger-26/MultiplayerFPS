using UnityEngine;
using Mirror;

namespace AudioUtils
{
    public struct AudioMessage : NetworkMessage
    {
        public string Id;

        public Vector3 Position;
        public NetworkTransform Parent;

        public float MaxDistance;
        public float MinDistance;
        public float Volume;
        public float Pitch;
        public float SpatialBlend;

        public int Priority;
    }
}