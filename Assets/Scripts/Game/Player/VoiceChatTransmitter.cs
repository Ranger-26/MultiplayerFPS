using Mirror;
using Steamworks;
using UnityEngine;

namespace Game.Player
{
    public class VoiceChatTransmitter : NetworkBehaviour
{
    public LayerMask PlayerMask;
    public AudioSource audioSource;
 
    void Update ()
    {
        if (isLocalPlayer && Input.GetKeyUp(KeyCode.V))
            SteamUser.StartVoiceRecording();
        else if (isLocalPlayer && Input.GetKeyDown(KeyCode.V))
            SteamUser.StopVoiceRecording();
 
 
        if(isLocalPlayer)
        {
            uint Compressed;
            uint Uncompressed;
            EVoiceResult ret = SteamUser.GetAvailableVoice(out Compressed);
            if(ret == EVoiceResult.k_EVoiceResultOK && Compressed > 1024)
            {
                Debug.Log(Compressed);
                byte[] DestBuffer = new byte[1024];
                uint BytesWritten;
                ret = SteamUser.GetVoice(true, DestBuffer, 1024, out BytesWritten);
                if(ret == EVoiceResult.k_EVoiceResultOK && BytesWritten > 0)
                {
                    Cmd_SendData(DestBuffer, BytesWritten);
                }
            }
        }
    }
 
    [Command (channel = 2)]
    void Cmd_SendData(byte[] data, uint size)
    {
        Debug.Log("Command");
        Collider[] cols = Physics.OverlapSphere(transform.position, 50, PlayerMask);
        for (int i = 0; i < cols.Length; i++)
        {
            if(cols[i].GetComponent<NetworkIdentity>())
            {
                Target_PlaySound(cols[i].GetComponent<NetworkIdentity>().connectionToClient, data, size);
            }
        }
    }
 
    [TargetRpc (channel = 2)]
    void Target_PlaySound(NetworkConnection connection, byte[] DestBuffer, uint BytesWritten)
    {
        Debug.Log("TARGET");
        byte[] DestBuffer2 = new byte[22050 * 2];
        uint BytesWritten2;
        EVoiceResult ret = SteamUser.DecompressVoice(DestBuffer, BytesWritten, DestBuffer2, (uint)DestBuffer2.Length, out BytesWritten2, 22050);
        if(ret == EVoiceResult.k_EVoiceResultOK && BytesWritten2 > 0)
        {
            audioSource.clip = AudioClip.Create(UnityEngine.Random.Range(100,1000000).ToString(), 22050, 1, 22050, false);
 
            float[] test = new float[22050];
            for (int i = 0; i < test.Length; ++i)
            {
                test[i] = (short)(DestBuffer2[i * 2] | DestBuffer2[i * 2 + 1] << 8) / 32768.0f;
            }
            audioSource.clip.SetData(test, 0);
            audioSource.Play();
        }
    }
}
 
 
}
