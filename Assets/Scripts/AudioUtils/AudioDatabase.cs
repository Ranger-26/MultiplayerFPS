using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace AudioUtils
{
    public class AudioDatabase : MonoBehaviour
    {
        private Dictionary<string, NetworkAudioClip> idsToAudio = new Dictionary<string, NetworkAudioClip>();

        public Dictionary<AudioClip, string> clipsToIds = new Dictionary<AudioClip, string>();

        public static AudioDatabase Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
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
            AudioClip[] clips = Resources.LoadAll<AudioClip>("");

            List<NetworkAudioClip> _clips = new List<NetworkAudioClip>();

            foreach (AudioClip au in clips)
            {
                _clips.Add(new NetworkAudioClip(au.name, au));
                //Debug.Log("Added " + au.name + " to clips");
            }

            foreach (var clip in _clips)
            {
                idsToAudio.Add(clip.AudioId, clip);
                clipsToIds.Add(clip.audioClip, clip.AudioId);
            }

            //Debug.Log($"Loaded {clips.Length} clips!");
        }

        public NetworkAudioClip TryGetClip(string id)
        {
            try
            {
                return idsToAudio[id];
            }
            catch
            {
                Debug.LogError($"Found no audio clip for id {id}");
            }

            return new NetworkAudioClip("NULL", null);
        }
    }
}