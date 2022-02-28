using UnityEngine;
using Mirror;

public static class AudioSystem
{
    public static void PlaySound(AudioClip _sound, Vector3 _position, float _maxDistance, float _volume, float _pitch, float _spatialBlend, int _priority)
    {
        GameObject soundObj = new GameObject("Sound");
        AudioSource au = soundObj.AddComponent<AudioSource>();
        au.playOnAwake = false;
        au.clip = _sound;
        au.maxDistance = _maxDistance;
        au.volume = _volume;
        au.pitch = _pitch;
        au.spatialBlend = _spatialBlend;
        au.priority = _priority;
        DestroyAfter des = soundObj.AddComponent<DestroyAfter>();
        des.Timer = _sound.length + 1f;
        soundObj.transform.position = _position;
        au.Play();
    }
}
