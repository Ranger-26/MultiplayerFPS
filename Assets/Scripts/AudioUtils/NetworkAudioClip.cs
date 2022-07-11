using UnityEngine;

namespace AudioUtils
{
    [System.Serializable]
    public struct NetworkAudioClip
    {
        public string AudioId;

        public AudioClip audioClip;

        public NetworkAudioClip(string id, AudioClip au)
        {
            AudioId = id;
            audioClip = au;
        }
    }
}