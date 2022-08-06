using Mirror;
using UnityEngine;

public class LocalPlayerIdentifier : NetworkBehaviour
{
    public static GameObject LocalPlayer;

    private void Start()
    {
        if (isLocalPlayer)
        {
            LocalPlayer = gameObject;
            gameObject.tag = "Player";
        }
        else
        {
            gameObject.tag = "Other Player";
            enabled = false;
        }
    }
}
