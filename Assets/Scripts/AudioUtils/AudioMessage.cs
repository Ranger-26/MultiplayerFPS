using UnityEngine;
using Mirror;

namespace AudioUtils
{
    public struct AudioMessage : NetworkMessage
    {
        public string Id;

        public Vector3 Position;

        public float MaxDistance;
        public float MinDistance;
        public float Volume;
        public float Pitch;
        public float SpatialBlend;

        public int Priority;
    }
}