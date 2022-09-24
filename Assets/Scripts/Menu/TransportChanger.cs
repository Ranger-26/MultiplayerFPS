using kcp2k;
using Menu;
using Mirror;
using Mirror.FizzySteam;
using Networking;
using UnityEngine;

[RequireComponent(typeof(NetworkManagerScp))]
public class TransportChanger : MonoBehaviour
{
    NetworkManagerScp scp;

    FizzySteamworks fizzy;
    KcpTransport kcp;

    private void Awake()
    {
        scp = GetComponent<NetworkManagerScp>();
        fizzy = GetComponent<FizzySteamworks>();
        kcp = GetComponent<KcpTransport>();
    }

    private void Start()
    {
        UpdateTransport(Settings.Current.Transport);
    }

    public void UpdateTransport(int v)
    {
        switch (v)
        {
            case 0:
                scp.transport = kcp;
                break;
            case 1:
                scp.transport = fizzy;
                break;
            default:
                scp.transport = fizzy;
                break;
        }
    }
}
