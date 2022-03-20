using UnityEngine;
using Mirror;

public class DisableOnRemote : MonoBehaviour
{
    NetworkIdentity ni;

    private void Awake()
    {
        ni = GetComponentInParent<NetworkIdentity>();
    }

    private void Start()
    {
        if (!ni.hasAuthority)
        {
            gameObject.SetActive(false);
        }
        else
        {
            enabled = false;
        }
    }
}
