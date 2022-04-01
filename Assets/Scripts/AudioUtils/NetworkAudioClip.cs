using UnityEngine;

namespace AudioUtils
{
    [CreateAssetMenu(fileName = "AudioClip", menuName = "Database Audio Clip")]
    public class NetworkAudioClip : ScriptableObject
    {
        public AudioId AudioId;

        public AudioClip AudioClip;
    }
}