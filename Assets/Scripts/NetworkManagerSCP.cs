using UnityEngine;
using Mirror;

public class NetworkManagerSCP : NetworkManager
{
    public string PlayerList;

    public string AttackerSpawn;
    public string DefenderSpawn;

    public override void OnStartServer()
    {
        ServerChangeScene(onlineScene);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab, GameObject.Find(PlayerList).transform);

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
