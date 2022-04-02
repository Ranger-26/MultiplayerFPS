using Mirror;
using Networking;
using UnityEngine;

namespace AudioUtils
{
    public static class AudioSystem
    {
        public static void PlaySound(this AudioClip _sound, Vector3 _position, float _maxDistance, float _volume, float _pitch, float _spatialBlend, int _priority)
        {
            GameObject soundObj = new GameObject("Sound", typeof(AudioSource), typeof(DestroyAfter));
            AudioSource au = soundObj.GetComponent<AudioSource>();
            DestroyAfter des = soundObj.GetComponent<DestroyAfter>();
            soundObj.transform.position = _position;
            au.playOnAwake = false;
            au.clip = _sound;
            au.maxDistance = _maxDistance;
            au.volume = _volume;
            au.pitch = _pitch;
            au.spatialBlend = _spatialBlend;
            au.priority = _priority;
            des.Timer = _sound.length + 1f;
            au.rolloffMode = AudioRolloffMode.Linear;
            au.Play();
        }

        public static void PlaySound(this AudioClip _sound, Transform _parent, float _maxDistance, float _volume, float _pitch, float _spatialBlend, int _priority)
        {
            GameObject soundObj = new GameObject("Sound", typeof(AudioSource), typeof(DestroyAfter));
            AudioSource au = soundObj.GetComponent<AudioSource>();
            DestroyAfter des = soundObj.GetComponent<DestroyAfter>();
            soundObj.transform.parent = _parent;
            soundObj.transform.localPosition = Vector3.zero;
            au.playOnAwake = false;
            au.clip = _sound;
            au.maxDistance = _maxDistance;
            au.volume = _volume;
            au.pitch = _pitch;
            au.spatialBlend = _spatialBlend;
            au.priority = _priority;
            des.Timer = _sound.length + 1f;
            au.rolloffMode = AudioRolloffMode.Linear;
            au.Play();
        }
        
        public static void OnClientReceiveAudioMessage(AudioMessage message)
        { 
            AudioDatabase.Instance.TryGetClip(message.id)?.AudioClip.PlaySound(message.position, message.maxDistance, message.volume,
                message.pitch, message.spatialBlend, message.priority);
        }
        
        public static void OnServerRecieveAudioMessage(NetworkConnection conn, AudioMessage message)
        {
            NetworkServer.SendToAll(message, Channels.Unreliable);
        }
        
        public static void RegisterHandlers()
        {
            Debug.Log("Registered Audio Network Handlers!");
            NetworkClient.RegisterHandler<AudioMessage>(OnClientReceiveAudioMessage);
            NetworkServer.RegisterHandler<AudioMessage>(OnServerRecieveAudioMessage);
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            NetworkManagerScp.OnClientJoin += RegisterHandlers;
        }
        
    }
}

