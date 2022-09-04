using Menu;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioUtils
{
    public static class AudioSystem
    {
        static AudioMixerGroup mixer;

        public static void PlaySound(this AudioClip Sound, Vector3 Position, Transform Parent = null, float MinDistance = 0.0f, float MaxDistance = 10f, float Volume = 1f, float Pitch = 1f, float SpatialBlend = 1f, int Priority = 0)
        {
            GameObject SoundObject = new GameObject("Sound", typeof(AudioSource), typeof(DestroyAfter));
            AudioSource Clip = SoundObject.GetComponent<AudioSource>();
            DestroyAfter AutoDestroy = SoundObject.GetComponent<DestroyAfter>();
            SoundObject.transform.position = Position;

            if (mixer == null)
                mixer = SettingsMenu.Instance.audioMixer.FindMatchingGroups("Master/Effects")[0];

            if (Parent != null)
                SoundObject.transform.SetParent(Parent);

            AutoDestroy.Timer = Sound.length;

            Clip.playOnAwake = false;
            Clip.clip = Sound;
            Clip.maxDistance = MaxDistance;
            Clip.volume = Volume;
            Clip.pitch = Pitch;
            Clip.spatialBlend = SpatialBlend;
            Clip.priority = Priority;
            Clip.rolloffMode = AudioRolloffMode.Linear;
            Clip.minDistance = MinDistance;
            Clip.outputAudioMixerGroup = mixer;
            Clip.Play();
        }

        public static void NetworkPlaySound(this AudioClip Sound, Vector3 Position, NetworkTransform Parent = default, float MinDistance = 0.0f, float MaxDistance = 10f, float Volume = 1f, float Pitch = 1f, float SpatialBlend = 1f, int Priority = 0)
        {
            AudioMessage message = new AudioMessage()
            {
                Id = AudioDatabase.Instance.clipsToIds[Sound],
                Position = Position,
                Parent = Parent,
                MaxDistance = MaxDistance,
                MinDistance = MinDistance,
                Volume = Volume,
                Pitch = Pitch,
                SpatialBlend = SpatialBlend,
                Priority = Priority
            };
            OnClientReceiveAudioMessage(message);
            NetworkClient.Send(message, Channels.Unreliable);
        }

        public static void OnClientReceiveAudioMessage(AudioMessage message)
        {
            Transform ParentTransform = message.Parent != null || message.Parent != default ? message.Parent.transform : null;
            AudioDatabase.Instance.TryGetClip(message.Id).audioClip.PlaySound(Position: message.Position, Parent: ParentTransform, MaxDistance: message.MaxDistance, MinDistance: message.MinDistance, Volume: message.Volume,
                Pitch: message.Pitch, SpatialBlend: message.SpatialBlend, Priority: message.Priority);
        }

        public static void OnServerRecieveAudioMessage(NetworkConnection conn, AudioMessage message)
        {
            NetworkUtils.SendToAllExceptOne(message, conn as NetworkConnectionToClient, Channels.Unreliable);
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

