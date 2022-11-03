using System;
using Mirror;
using UnityEngine;

namespace Networking
{
    public static class NetworkUtils
    {
        public static void SendToAllExceptOne<T>(T message, NetworkConnectionToClient exclude, int channelId = Channels.Reliable, bool sendToReadyOnly = false)
            where T : struct, NetworkMessage
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("Can not send using NetworkServer.SendToAll<T>(T msg) because NetworkServer is not active");
                return;
            }

            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (sendToReadyOnly && !conn.isReady || conn == exclude)
                    continue;
                
                conn.Send(message, channelId);
            }
        }

        public static void SpawnObject(GameObject obj, NetworkConnectionToClient exclude)
        {
            
        }
    }
}