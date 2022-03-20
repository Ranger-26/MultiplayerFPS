using UnityEngine;
using Mirror;

public class ViewModel : MonoBehaviour
{
    NetworkIdentity ni;

    private void Awake()
    {
        ni = GetComponentInParent<NetworkIdentity>();
    }

    private void Start()
    {
        if (ni.hasAuthority)
        {
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.gameObject.layer = 7;
            }
        }
    }
}
