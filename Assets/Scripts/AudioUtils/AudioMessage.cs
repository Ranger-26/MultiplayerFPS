using UnityEngine;
using Mirror;

namespace AudioUtils
{
    public struct AudioMessage : NetworkMessage
    {
        public string id;

        public Vector3 Position;
        public NetworkTransform Parent;

        public float maxDistance;
        public float volume;
        public float pitch;
        public float spatialBlend;

        public int priority;
    }
}