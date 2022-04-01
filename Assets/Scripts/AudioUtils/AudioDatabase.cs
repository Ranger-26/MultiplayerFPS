using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace AudioUtils
{
    public class AudioDatabase : MonoBehaviour
    {
        private Dictionary<AudioId, NetworkAudioClip> idsToAudio = new Dictionary<AudioId, NetworkAudioClip>();

        public static AudioDatabase Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LoadClips();
        }

        public void LoadClips()
        {
            NetworkAudioClip[] clips = Resources.LoadAll<NetworkAudioClip>("AudioClips");
            foreach (var clip in clips)
            {
                idsToAudio.Add(clip.AudioId, clip);
            }
            Debug.Log($"Loaded {clips.Length} clips!");
        }

        public NetworkAudioClip TryGetClip(AudioId id)
        {
            if (idsToAudio[id] == null)
            {
                Debug.LogError($"Found no audio clip for id {id}");
                return null;
            }
            return idsToAudio[id];
        }
    }
}