using UnityEngine;
using Mirror;

public class DisableOnRemote : MonoBehaviour
{
    public bool Reverse = false;

    NetworkIdentity ni;

    private void Awake()
    {
        ni = GetComponentInParent<NetworkIdentity>();
    }

    private void Start()
    {
        if (!ni.hasAuthority || (Reverse && ni.hasAuthority))
        {
            gameObject.SetActive(false);
        }
        else
        {
            enabled = false;
        }
    }
}
